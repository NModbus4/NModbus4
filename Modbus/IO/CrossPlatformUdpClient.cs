using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Modbus.IO
{
    using Unme.Common;

    internal abstract class CrossPlatformUdpClient : IDisposable
    {
        private UdpClient _udpClient;

        public CrossPlatformUdpClient(UdpClient udpClient)
        {
            if (udpClient == null)
                throw new ArgumentNullException("udpClient");

            _udpClient = udpClient;
        }

        public abstract int ReadTimeout { get; set; }

        public abstract int WriteTimeout { get; set; }

        protected UdpClient UdpClient
        {
            get { return _udpClient; }
        }

        protected List<byte> Buffer { get; set; }

        public static CrossPlatformUdpClient Create(UdpClient udpClient)
        {
#if WindowsCE
            return new CompactUdpClient(udpClient);
#else
            return new FullUdpClient(udpClient);
#endif
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal int Read(byte[] buffer, int offset, int count)
        {
            if (Buffer == null || Buffer.Count == 0)
                Buffer = Read();

            if (Buffer.Count < count)
                throw new IOException("Not enough bytes in the datagram.");

            Buffer.CopyTo(0, buffer, offset, count);
            Buffer.RemoveRange(0, count);

            return count;
        }

        /// <summary>
        ///     This method facilitates unit testing.
        /// </summary>
        internal virtual List<byte> Read()
        {
            IPEndPoint remoteIpEndPoint = null;

            return UdpClient.Receive(ref remoteIpEndPoint).ToList();
        }

        internal void Write(byte[] buffer, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (count > buffer.Length)
                throw new ArgumentOutOfRangeException("count",
                    "Argument count cannot be greater than the length of buffer.");

            UdpClient.Send(buffer, count);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                DisposableUtility.Dispose(ref _udpClient);
        }
    }
}