namespace Modbus.IO
{
    using System;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    ///     No timeout support for compact framework.
    /// </summary>
    internal class CompactUdpClient : CrossPlatformUdpClient
    {
        public CompactUdpClient(UdpClient udpClient)
            : base(udpClient)
        {
        }

        public override int ReadTimeout
        {
            get { return Timeout.Infinite; }
            set { throw new InvalidOperationException(Resources.TimeoutNotSupported); }
        }

        public override int WriteTimeout
        {
            get { return Timeout.Infinite; }
            set { throw new InvalidOperationException(Resources.TimeoutNotSupported); }
        }
    }
}