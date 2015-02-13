using System;
using System.Net.Sockets;
using System.Threading;
using Modbus.IO;

namespace Modbus.UnitTests.IO
{
    using NUnit.Framework;

    [TestFixture]
    public class CompactUdpClientFixture
    {
        [Test]
        public void ReadTimeout()
        {
            var client = new CompactUdpClient(new UdpClient());
            Assert.AreEqual(Timeout.Infinite, client.ReadTimeout);
            Assert.Throws<InvalidOperationException>(() => client.ReadTimeout = 1000);
        }

        [Test]
        public void WriteTimeout()
        {
            var client = new CompactUdpClient(new UdpClient());
            Assert.AreEqual(Timeout.Infinite, client.WriteTimeout);
            Assert.Throws<InvalidOperationException>(() => client.WriteTimeout = 1000);
        }
    }
}