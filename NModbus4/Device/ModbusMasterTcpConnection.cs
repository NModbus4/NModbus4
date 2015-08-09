namespace Modbus.Device
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    using IO;
    using Message;

    using Unme.Common;

    /// <summary>
    /// Represents an incoming connection from a Modbus master. Contains the slave's logic to process the connection.
    /// </summary>
    internal class ModbusMasterTcpConnection : ModbusDevice, IDisposable
    {
        private readonly TcpClient _client;
        private readonly string _endPoint;
        private readonly Stream _stream;
        private readonly ModbusTcpSlave _slave;
        private readonly AsyncCallback _readHeaderCompletedCallback;
        private readonly AsyncCallback _readFrameCompletedCallback;
        private readonly AsyncCallback _writeCompletedCallback;

        private readonly byte[] _mbapHeader = new byte[6];
        private byte[] _messageFrame;

        public ModbusMasterTcpConnection(TcpClient client, ModbusTcpSlave slave)
            : base(new ModbusIpTransport(new TcpClientAdapter(client)))
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (slave == null)
                throw new ArgumentNullException("slave");

            _client = client;
            _endPoint = client.Client.RemoteEndPoint.ToString();
            _stream = client.GetStream();
            _slave = slave;
            _readHeaderCompletedCallback = ReadHeaderCompleted;
            _readFrameCompletedCallback = ReadFrameCompleted;
            _writeCompletedCallback = WriteCompleted;

            Debug.WriteLine("Creating new Master connection at IP:{0}", EndPoint);
            Debug.WriteLine("Begin reading header.");

            Stream.BeginRead(_mbapHeader, 0, 6, _readHeaderCompletedCallback, null);
        }

        /// <summary>
        ///     Occurs when a Modbus master TCP connection is closed.
        /// </summary>
        public event EventHandler<TcpConnectionEventArgs> ModbusMasterTcpConnectionClosed;

        public string EndPoint
        {
            get { return _endPoint; }
        }

        public Stream Stream
        {
            get { return _stream; }
        }

        public TcpClient TcpClient
        {
            get { return _client; }
        }

        private void ReadHeaderCompleted(IAsyncResult ar)
        {
            Debug.WriteLine("Read header completed.");

            CatchExceptionAndRemoveMasterEndPoint(ar, (thisRef, asyncResult) =>
            {
                // this is the normal way a master closes its connection
                if (thisRef.Stream.EndRead(asyncResult) == 0)
                {
                    Debug.WriteLine("0 bytes read, Master has closed Socket connection.");
                    thisRef.ModbusMasterTcpConnectionClosed.Raise(thisRef, new TcpConnectionEventArgs(thisRef.EndPoint));
                    return;
                }

                Debug.WriteLine("MBAP header: {0}", string.Join(", ", thisRef._mbapHeader));
                ushort frameLength = (ushort)IPAddress.HostToNetworkOrder(BitConverter.ToInt16(thisRef._mbapHeader, 4));
                Debug.WriteLine("{0} bytes in PDU.", frameLength);
                thisRef._messageFrame = new byte[frameLength];

                thisRef.Stream.BeginRead(thisRef._messageFrame, 0, frameLength, thisRef._readFrameCompletedCallback, null);
            }, EndPoint);
        }

        private void ReadFrameCompleted(IAsyncResult ar)
        {
            CatchExceptionAndRemoveMasterEndPoint(ar, (thisRef, asyncResult) =>
            {
                Debug.WriteLine("Read Frame completed {0} bytes", thisRef.Stream.EndRead(asyncResult));
                byte[] frame = thisRef._mbapHeader.Concat(thisRef._messageFrame).ToArray();
                Debug.WriteLine("RX: {0}", string.Join(", ", frame));

                IModbusMessage request =
                    ModbusMessageFactory.CreateModbusRequest(frame.Slice(6, frame.Length - 6).ToArray());
                request.TransactionId = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 0));

                // perform action and build response
                IModbusMessage response = thisRef._slave.ApplyRequest(request);
                response.TransactionId = request.TransactionId;

                // write response
                byte[] responseFrame = thisRef.Transport.BuildMessageFrame(response);
                Debug.WriteLine("TX: {0}", string.Join(", ", responseFrame));
                thisRef.Stream.BeginWrite(responseFrame, 0, responseFrame.Length, thisRef._writeCompletedCallback, null);
            }, EndPoint);
        }

        private void WriteCompleted(IAsyncResult ar)
        {
            Debug.WriteLine("End write.");

            CatchExceptionAndRemoveMasterEndPoint(ar, (thisRef, asyncResult) =>
            {
                thisRef.Stream.EndWrite(asyncResult);
                Debug.WriteLine("Begin reading another request.");
                thisRef.Stream.BeginRead(thisRef._mbapHeader, 0, 6, thisRef._readHeaderCompletedCallback, null);
            }, EndPoint);
        }

        /// <summary>
        ///     Catches all exceptions thrown when action is executed and removes the master end point.
        ///     The exception is ignored when it simply signals a master closing its connection.
        /// </summary>
        private void CatchExceptionAndRemoveMasterEndPoint(IAsyncResult ar, Action<ModbusMasterTcpConnection, IAsyncResult> action, string endPoint)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (endPoint == null)
                throw new ArgumentNullException("endPoint");
            if (endPoint == string.Empty)
                throw new ArgumentException(Resources.EmptyEndPoint);

            try
            {
                action(this, ar);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception processing request: [{0}] {1}", ex.GetType().Name, ex.Message);

                // This will typically result in the exception being unhandled, which will terminate the thread pool thread and
                // thereby the process, depending on the process's configuration. Such a crash would cause all connections to be
                // dropped, even if the slave were restarted.
                // Otherwise, the request is discarded and the slave awaits the next message. If the master is unable to synchronize
                //the frame, it can drop the connection.
                if (!(ex is IOException || ex is FormatException || ex is ObjectDisposedException))
                    throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
