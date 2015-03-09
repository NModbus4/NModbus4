namespace Modbus.Device
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Net.Sockets;
    using System.Diagnostics;
    using System.IO;

    using IO;

    /// <summary>
    ///     Modbus TCP slave device.
    /// </summary>
    public class ModbusTcpSlave : ModbusSlave
    {
        private readonly object _serverLock = new object();

        private readonly ConcurrentDictionary<string, ModbusMasterTcpConnection> _masters =
            new ConcurrentDictionary<string, ModbusMasterTcpConnection>();

        private TcpListener _server;
        private System.Timers.Timer _timer;

        private ModbusTcpSlave(byte unitId, TcpListener tcpListener)
            : base(unitId, new EmptyTransport())
        {
            if (tcpListener == null)
                throw new ArgumentNullException("tcpListener");

            _server = tcpListener;
        }

        private ModbusTcpSlave(byte unitId, TcpListener tcpListener, double timeInterval)
            : base(unitId, new EmptyTransport())
        {
            if (tcpListener == null)
                throw new ArgumentNullException("tcpListener");

            _server = tcpListener;

            //default timeout(ms) for closing TcpClient when it isn't active
            _timer = new System.Timers.Timer(timeInterval);
            _timer.Elapsed += OnTimer;
            _timer.Enabled = true;
        }


        private void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var master in _masters.ToList())
            {
                if (IsSocketConnected(master.Value.TcpClient.Client) == false)
                {
                    Debug.WriteLine("Removed Master {0}", master.Value.EndPoint);

                    master.Value.Dispose();
                }
            }
        }

        /// <summary>
        /// The time to wait for a response, in microseconds.
        /// </summary>
        private const int TimeWaitResponse = 1000;

        /// <summary>
        /// Check a socket (connected=true/disconnected=false)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool IsSocketConnected(Socket s)
        {
            bool poll = s.Poll(TimeWaitResponse, SelectMode.SelectRead);
            bool available = (s.Available == 0);

            return poll && available;
        }

        /// <summary>
        ///     Gets the Modbus TCP Masters connected to this Modbus TCP Slave.
        /// </summary>
        public ReadOnlyCollection<TcpClient> Masters
        {
            get
            {
                return new ReadOnlyCollection<TcpClient>(_masters.Values.Select(mc => mc.TcpClient).ToList());
            }
        }

        /// <summary>
        ///     Gets the server.
        /// </summary>
        /// <value>The server.</value>
        /// <remarks>
        ///     This property is not thread safe, it should only be consumed within a lock.
        /// </remarks>
        private TcpListener Server
        {
            get
            {
                if (_server == null)
                    throw new ObjectDisposedException("Server");

                return _server;
            }
        }

        /// <summary>
        ///     Modbus TCP slave factory method.
        /// </summary>
        public static ModbusTcpSlave CreateTcp(byte unitId, TcpListener tcpListener)
        {
            return new ModbusTcpSlave(unitId, tcpListener);
        }

        /// <summary>
        ///     Modbus TCP slave factory method with timeInterval
        /// </summary>
        public static ModbusTcpSlave CreateTcp(byte unitId, TcpListener tcpListener, double timeInterval)
        {
            return new ModbusTcpSlave(unitId, tcpListener, timeInterval);
        }

        /// <summary>
        ///     Start slave listening for requests.
        /// </summary>
        public override void Listen()
        {
            Debug.WriteLine("Start Modbus Tcp Server.");

            lock (_serverLock)
            {
                try
                {
                    Server.Start();

                    // use Socket async API for compact framework compat
                    Server.Server.BeginAccept(AcceptCompleted, this);
                }
                catch (ObjectDisposedException)
                {
                    // this happens when the server stops
                }
            }
        }

        private void OnMasterConnectionClosedHandler(object sender, TcpConnectionEventArgs e)
        {
            ModbusMasterTcpConnection connection;
            if (!_masters.TryRemove(e.EndPoint, out connection))
            {
                var msg = string.Format(
                    CultureInfo.InvariantCulture,
                    "EndPoint {0} cannot be removed, it does not exist.",
                    e.EndPoint);

                throw new ArgumentException(msg);
            }

            Debug.WriteLine("Removed Master {0}", e.EndPoint);
        }

        internal void AcceptCompleted(IAsyncResult ar)
        {
            ModbusTcpSlave slave = (ModbusTcpSlave) ar.AsyncState;

            try
            {
                try
                {
                    // use Socket async API for compact framework compat
                    Socket socket = null;
                    lock (_serverLock)
                    {
                        if (_server == null) // Checks for disposal to an otherwise unnecessary exception (which is slow and hinders debugging).
                            return;
                        socket = Server.Server.EndAccept(ar);
                    }

                    TcpClient client = new TcpClient {Client = socket};
                    var masterConnection = new ModbusMasterTcpConnection(client, slave);
                    masterConnection.ModbusMasterTcpConnectionClosed += OnMasterConnectionClosedHandler;

                    _masters.TryAdd(client.Client.RemoteEndPoint.ToString(), masterConnection);

                    Debug.WriteLine("Accept completed.");
                }
                catch (IOException ex)
                {
                    // Abandon the connection attempt and continue to accepting the next connection.
                    Debug.WriteLine("Accept failed: " + ex.Message);
                }

                // Accept another client
                // use Socket async API for compact framework compat
                lock (_serverLock)
                    Server.Server.BeginAccept(AcceptCompleted, slave);
            }
            catch (ObjectDisposedException)
            {
                // this happens when the server stops
            }
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        /// <remarks>Dispose is thread-safe.</remarks>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // double-check locking
                if (_server != null)
                {
                    lock (_serverLock)
                    {
                        if (_server != null)
                        {
                            _server.Stop();
                            _server = null;

                            foreach (var key in _masters.Keys)
                            {
                                ModbusMasterTcpConnection connection;
                                if (_masters.TryRemove(key, out connection))
                                {
                                    connection.ModbusMasterTcpConnectionClosed -= OnMasterConnectionClosedHandler;
                                    connection.Dispose();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
