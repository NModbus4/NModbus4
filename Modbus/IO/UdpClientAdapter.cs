using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Modbus.IO
{
    using Unme.Common;

    /// <summary>
    ///     Concrete Implementor - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    internal class UdpClientAdapter : IStreamResource
    {
        // strategy for cross platform r/w 
        private CrossPlatformUdpClient _udpClient;

        public UdpClientAdapter(UdpClient udpClient)
        {
            if (udpClient == null)
                throw new ArgumentNullException("udpClient");

            _udpClient = CrossPlatformUdpClient.Create(udpClient);
        }

        public CrossPlatformUdpClient Client
        {
            get { return _udpClient; }
        }

        public int InfiniteTimeout
        {
            get { return Timeout.Infinite; }
        }

        public int ReadTimeout
        {
            get { return Client.ReadTimeout; }
            set { Client.ReadTimeout = value; }
        }

        public int WriteTimeout
        {
            get { return Client.WriteTimeout; }
            set { Client.WriteTimeout = value; }
        }

        public void DiscardInBuffer()
        {
            // no-op
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "Argument offset must be greater than or equal to 0.");
            if (offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset",
                    "Argument offset cannot be greater than the length of buffer.");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Argument count must be greater than or equal to 0.");
            if (count > buffer.Length - offset)
                throw new ArgumentOutOfRangeException("count",
                    "Argument count cannot be greater than the length of buffer minus offset.");

            return Client.Read(buffer, offset, count);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "Argument offset must be greater than or equal to 0.");
            if (offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset",
                    "Argument offset cannot be greater than the length of buffer.");

            Client.Write(buffer.Skip(offset).ToArray(), count);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                DisposableUtility.Dispose(ref _udpClient);
        }
    }
}