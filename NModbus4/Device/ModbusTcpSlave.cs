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
    using System.Timers;

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
        private Timer _timer;
        private const int TimeWaitResponse = 1000;

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
            _timer = new Timer(timeInterval);
            _timer.Elapsed += OnTimer;
            _timer.Enabled = true;
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
        ///     Creates ModbusTcpSlave with timer which polls connected clients every <paramref name="pollInterval"/>
        /// milliseconds on that they are connected.
        /// </summary>
        public static ModbusTcpSlave CreateTcp(byte unitId, TcpListener tcpListener, double pollInterval)
        {
            return new ModbusTcpSlave(unitId, tcpListener, pollInterval);
        }

        private static bool IsSocketConnected(Socket socket)
        {
            bool poll = socket.Poll(TimeWaitResponse, SelectMode.SelectRead);
            bool available = (socket.Available == 0);
            return poll && available;
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
                    Server.Server.BeginAccept(state => AcceptCompleted(state), this);
                }
                catch (ObjectDisposedException)
                {
                    // this happens when the server stops
                }
            }
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            foreach (var master in _masters.ToList())
            {
                if (IsSocketConnected(master.Value.TcpClient.Client) == false)
                {
                    master.Value.Dispose();
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

        private static void AcceptCompleted(IAsyncResult ar)
        {
            ModbusTcpSlave slave = (ModbusTcpSlave)ar.AsyncState;

            try
            {
                try
                {
                    // use Socket async API for compact framework compat
                    Socket socket = null;
                    lock (slave._serverLock)
                    {
                        if (slave._server == null) // Checks for disposal to an otherwise unnecessary exception (which is slow and hinders debugging).
                            return;
                        socket = slave.Server.Server.EndAccept(ar);
                    }

                    TcpClient client = new TcpClient {Client = socket};
                    var masterConnection = new ModbusMasterTcpConnection(client, slave);
                    masterConnection.ModbusMasterTcpConnectionClosed += slave.OnMasterConnectionClosedHandler;

                    slave._masters.TryAdd(client.Client.RemoteEndPoint.ToString(), masterConnection);

                    Debug.WriteLine("Accept completed.");
                }
                catch (IOException ex)
                {
                    // Abandon the connection attempt and continue to accepting the next connection.
                    Debug.WriteLine("Accept failed: " + ex.Message);
                }

                // Accept another client
                // use Socket async API for compact framework compat
                lock (slave._serverLock)
                {
                    slave.Server.Server.BeginAccept(state => AcceptCompleted(state), slave);
                }
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

                            if (_timer != null)
                            {
                                _timer.Dispose();
                                _timer = null;
                            }

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
