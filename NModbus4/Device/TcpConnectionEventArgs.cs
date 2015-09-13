namespace Modbus.Device
{
    using System;

    /// <summary>
    ///
    /// </summary>
    internal class TcpConnectionEventArgs : EventArgs
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="endPoint"></param>
        public TcpConnectionEventArgs(string endPoint)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            if (endPoint == string.Empty)
            {
                throw new ArgumentException(Resources.EmptyEndPoint);
            }

            EndPoint = endPoint;
        }

        public string EndPoint { get; set; }
    }
}
