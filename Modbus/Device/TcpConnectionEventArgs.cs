using System;

namespace Modbus.Device
{
    using Unme.Common;

    internal class TcpConnectionEventArgs : EventArgs
	{
		public TcpConnectionEventArgs(string endPoint)
		{
			if (endPoint == null)
				throw new ArgumentNullException("endPoint");
			if (endPoint.IsNullOrEmpty())
				throw new ArgumentException(Resources.EmptyEndPoint);

			EndPoint = endPoint;
		}

		public string EndPoint { get; set; }
	}
}
