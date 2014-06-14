using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Modbus.IO;
using Rhino.Mocks;

namespace Modbus.UnitTests.IO
{
    using NUnit.Framework;

    [TestFixture]
	public class CrossPlatformUdpClientFixture
	{
		[Test, ExpectedException(typeof(IOException))]
		public void Read_NotEnoughInBuffer()
		{
			var mocks = new MockRepository();
			var adapter = mocks.PartialMock<CrossPlatformUdpClient>(new UdpClient());

			var buffer = new byte[5];
			Expect.Call(adapter.Read())
				.Return(new List<byte>(new byte[] { 1, 2, 3, 4, 5 }));

			mocks.ReplayAll();

			// read first part of message
			Assert.AreEqual(4, adapter.Read(buffer, 0, 4));

			// read remainder... not enough in buffer
			Assert.AreEqual(2, adapter.Read(buffer, 3, 2));
		}

		[Test]
		public void Read_SingleMessageInTwoParts()
		{
			var mocks = new MockRepository();
			var adapter = mocks.PartialMock<CrossPlatformUdpClient>(new UdpClient());

			Expect.Call(adapter.Read())
				.Return(new List<byte>(new byte[] { 1, 2, 3, 4, 5 }));

			mocks.ReplayAll();

			var buffer = new byte[5];

			// read first part of message
			Assert.AreEqual(3, adapter.Read(buffer, 0, 3));

			// read remainder
			Assert.AreEqual(2, adapter.Read(buffer, 3, 2));

			Assert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, buffer);

			mocks.VerifyAll();
		}

		[Test]
		public void Read_TwoMessages()
		{
			var mocks = new MockRepository();
			var adapter = mocks.PartialMock<CrossPlatformUdpClient>(new UdpClient());

			// first datagram
			Expect.Call(adapter.Read())
				.Return(new List<byte>(new byte[] { 1 }));

			// second datagram
			Expect.Call(adapter.Read())
				.Return(new List<byte>(new byte[] { 2, 3, 4 }));

			mocks.ReplayAll();

			// read first datagram
			var buffer = new byte[1];
			Assert.AreEqual(1, adapter.Read(buffer, 0, 1));

			// read second datagram
			buffer = new byte[3];
			Assert.AreEqual(3, adapter.Read(buffer, 0, 3));

			Assert.AreEqual(new byte[] { 2, 3, 4 }, buffer);

			mocks.VerifyAll();
		}
	}
}
