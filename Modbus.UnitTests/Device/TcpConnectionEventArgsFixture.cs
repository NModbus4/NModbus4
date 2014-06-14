using System;
using Modbus.Device;

namespace Modbus.UnitTests.Device
{
    using NUnit.Framework;

    [TestFixture]
	public class TcpConnectionEventArgsFixture
	{
		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void TcpConnectionEventArgs_NullEndPoint()
		{
			new TcpConnectionEventArgs(null);
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void TcpConnectionEventArgs_EmptyEndPoint()
		{
			new TcpConnectionEventArgs(String.Empty);
		}

		[Test]
		public void TcpConnectionEventArgs()
		{
			var args = new TcpConnectionEventArgs("foo");

			Assert.AreEqual("foo", args.EndPoint);
		}
	}
}
