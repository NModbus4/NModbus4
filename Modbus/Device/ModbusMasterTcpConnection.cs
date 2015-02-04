using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Modbus.IO;
using Modbus.Message;

namespace Modbus.Device
{
    using System.Diagnostics;
    using Unme.Common;

    internal class ModbusMasterTcpConnection : ModbusDevice, IDisposable
    {
        private readonly TcpClient _client;
        private readonly string _endPoint;
        private readonly Stream _stream;
        private readonly ModbusTcpSlave _slave;

        private readonly byte[] _mbapHeader = new byte[6];
        private byte[] _messageFrame;

        public ModbusMasterTcpConnection(TcpClient client, ModbusTcpSlave slave)
            : base(new ModbusIpTransport(new TcpClientAdapter(client)))
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (slave == null)
                throw new ArgumentException("slave");

            _client = client;
            _endPoint = client.Client.RemoteEndPoint.ToString();
            _stream = client.GetStream();
            _slave = slave;
            Debug.WriteLine("Creating new Master connection at IP:{0}", EndPoint);

            Debug.WriteLine("Begin reading header.");
            Stream.BeginRead(_mbapHeader, 0, 6, ReadHeaderCompleted, null);
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

        internal void ReadHeaderCompleted(IAsyncResult ar)
        {
            Debug.WriteLine("Read header completed.");

            CatchExceptionAndRemoveMasterEndPoint(() =>
            {
                // this is the normal way a master closes its connection
                if (Stream.EndRead(ar) == 0)
                {
                    Debug.WriteLine("0 bytes read, Master has closed Socket connection.");
                    ModbusMasterTcpConnectionClosed.Raise(this, new TcpConnectionEventArgs(EndPoint));
                    return;
                }

                Debug.WriteLine("MBAP header: {0}", _mbapHeader.Join(", "));
                ushort frameLength = (ushort) IPAddress.HostToNetworkOrder(BitConverter.ToInt16(_mbapHeader, 4));
                Debug.WriteLine("{0} bytes in PDU.", frameLength);
                _messageFrame = new byte[frameLength];

                Stream.BeginRead(_messageFrame, 0, frameLength, ReadFrameCompleted, null);
            }, EndPoint);
        }

        internal void ReadFrameCompleted(IAsyncResult ar)
        {
            CatchExceptionAndRemoveMasterEndPoint(() =>
            {
                Debug.WriteLine("Read Frame completed {0} bytes", Stream.EndRead(ar));
                byte[] frame = _mbapHeader.Concat(_messageFrame).ToArray();
                Debug.WriteLine("RX: {0}", frame.Join(", "));

                IModbusMessage request =
                    ModbusMessageFactory.CreateModbusRequest(frame.Slice(6, frame.Length - 6).ToArray());
                request.TransactionId = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 0));

                // perform action and build response
                IModbusMessage response = _slave.ApplyRequest(request);
                response.TransactionId = request.TransactionId;

                // write response
                byte[] responseFrame = Transport.BuildMessageFrame(response);
                Debug.WriteLine("TX: {0}", responseFrame.Join(", "));
                Stream.BeginWrite(responseFrame, 0, responseFrame.Length, WriteCompleted, null);
            }, EndPoint);
        }

        internal void WriteCompleted(IAsyncResult ar)
        {
            Debug.WriteLine("End write.");

            CatchExceptionAndRemoveMasterEndPoint(() =>
            {
                Stream.EndWrite(ar);
                Debug.WriteLine("Begin reading another request.");
                Stream.BeginRead(_mbapHeader, 0, 6, ReadHeaderCompleted, null);
            }, EndPoint);
        }

        /// <summary>
        ///     Catches all exceptions thrown when action is executed and removes the master end point.
        ///     The exception is ignored when it simply signals a master closing its connection.
        /// </summary>
        internal void CatchExceptionAndRemoveMasterEndPoint(Action action, string endPoint)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (endPoint == null)
                throw new ArgumentNullException("endPoint");
            if (endPoint.IsNullOrEmpty())
                throw new ArgumentException("Argument endPoint cannot be empty.");

            try
            {
                action.Invoke();
            }
            catch (IOException ioe)
            {
                Debug.WriteLine("IOException encountered in ReadHeaderCompleted - {0}", ioe.Message);
                ModbusMasterTcpConnectionClosed.Raise(this, new TcpConnectionEventArgs(EndPoint));

                SocketException socketException = ioe.InnerException as SocketException;
                if (socketException != null && socketException.ErrorCode == Modbus.ConnectionResetByPeer)
                {
                    Debug.WriteLine("Socket Exception ConnectionResetByPeer, Master closed connection.");
                    return;
                }

                throw;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unexpected exception encountered: [{0}] {1}", e.GetType().Name, e.Message);
                throw;
            }
        }
    }
}