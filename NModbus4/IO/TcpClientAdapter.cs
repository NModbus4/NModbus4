namespace Modbus.IO
{
    using System;
    using System.Diagnostics;
    using System.Net.Sockets;
    using System.Threading;

    using Unme.Common;

    /// <summary>
    ///     Concrete Implementor - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    internal class TcpClientAdapter : IStreamResource
    {
        private TcpClient _tcpClient;

        /// <summary>
        ///
        /// </summary>
        /// <param name="tcpClient"></param>
        public TcpClientAdapter(TcpClient tcpClient)
        {
            Debug.Assert(tcpClient != null, "Argument tcpClient cannot be null.");

            _tcpClient = tcpClient;
        }

        /// <summary>
        ///
        /// </summary>
        public int InfiniteTimeout
        {
            get { return Timeout.Infinite; }
        }

        /// <summary>
        ///
        /// </summary>
        public int ReadTimeout
        {
            get { return _tcpClient.GetStream().ReadTimeout; }
            set { _tcpClient.GetStream().ReadTimeout = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public int WriteTimeout
        {
            get { return _tcpClient.GetStream().WriteTimeout; }
            set { _tcpClient.GetStream().WriteTimeout = value; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        public void Write(byte[] buffer,
                          int offset,
                          int size)
        {
            _tcpClient.GetStream().Write(buffer,
                                         offset,
                                         size);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public int Read(byte[] buffer,
                        int offset,
                        int size)
        {
            return _tcpClient.GetStream().Read(buffer,
                                               offset,
                                               size);
        }

        /// <summary>
        ///
        /// </summary>
        public void DiscardInBuffer()
        {
            _tcpClient.GetStream().Flush();
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposableUtility.Dispose(ref _tcpClient);
            }
        }
    }
}
