using System;
using System.Net.Sockets;
using Modbus.IO;

namespace Modbus.UnitTests.IO
{
    using NUnit.Framework;

    [TestFixture]
    public class UdpClientAdapterFixture
    {
        [Test]
        public void Read_ArgumentValidation()
        {
            var adapter = new UdpClientAdapter(new UdpClient());

            // buffer
            Assert.Throws<ArgumentNullException>(() => adapter.Read(null, 1, 1));

            // offset
            Assert.Throws<ArgumentOutOfRangeException>(() => adapter.Read(new byte[2], -1, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => adapter.Read(new byte[2], 3, 3));

            Assert.Throws<ArgumentOutOfRangeException>(() => adapter.Read(new byte[2], 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => adapter.Read(new byte[2], 1, 2));
        }

        [Test]
        public void Write_ArgumentValidation()
        {
            var adapter = new UdpClientAdapter(new UdpClient());

            // buffer 
            Assert.Throws<ArgumentNullException>(() => adapter.Write(null, 1, 1));

            // offset
            Assert.Throws<ArgumentOutOfRangeException>(() => adapter.Write(new byte[2], -1, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => adapter.Write(new byte[2], 3, 3));
        }
    }
}