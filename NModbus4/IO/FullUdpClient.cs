#if !WindowsCE

namespace Modbus.IO
{
    using System.Net.Sockets;

    internal class FullUdpClient : CrossPlatformUdpClient
    {
        public FullUdpClient(UdpClient udpClient)
            : base(udpClient)
        {
        }

        public override int ReadTimeout
        {
            get { return UdpClient.Client.ReceiveTimeout; }
            set { UdpClient.Client.ReceiveTimeout = value; }
        }

        public override int WriteTimeout
        {
            get { return UdpClient.Client.SendTimeout; }
            set { UdpClient.Client.SendTimeout = value; }
        }
    }
}

#endif