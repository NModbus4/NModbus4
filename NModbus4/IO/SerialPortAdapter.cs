namespace Modbus.IO
{
    using System;
    using System.Diagnostics;
    using System.IO.Ports;

    using Unme.Common;

    /// <summary>
    ///     Concrete Implementor - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    internal class SerialPortAdapter : IStreamResource
    {
        private SerialPort _serialPort;

        /// <summary>
        ///
        /// </summary>
        /// <param name="serialPort"></param>
        public SerialPortAdapter(SerialPort serialPort)
        {
            Debug.Assert(serialPort != null, "Argument serialPort cannot be null.");

            _serialPort = serialPort;
            _serialPort.NewLine = Modbus.NewLine;
        }

        /// <summary>
        ///
        /// </summary>
        public int InfiniteTimeout
        {
            get { return SerialPort.InfiniteTimeout; }
        }

        /// <summary>
        ///
        /// </summary>
        public int ReadTimeout
        {
            get { return _serialPort.ReadTimeout; }
            set { _serialPort.ReadTimeout = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public int WriteTimeout
        {
            get { return _serialPort.WriteTimeout; }
            set { _serialPort.WriteTimeout = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public void DiscardInBuffer()
        {
            _serialPort.DiscardInBuffer();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Read(byte[] buffer,
                        int offset,
                        int count)
        {
            return _serialPort.Read(buffer,
                                    offset,
                                    count);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void Write(byte[] buffer,
                          int offset,
                          int count)
        {
            _serialPort.Write(buffer,
                              offset,
                              count);
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
                DisposableUtility.Dispose(ref _serialPort);
            }
        }
    }
}
