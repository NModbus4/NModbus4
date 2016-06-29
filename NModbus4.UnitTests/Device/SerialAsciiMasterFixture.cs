using System;
using System.Net.Sockets;
using Modbus.Device;
using Modbus.IO;
using Xunit;

namespace Modbus.UnitTests.Device
{
    public class SerialAsciiMasterFixture : ModbusMasterFixture
    {
        protected override ModbusMaster Master => ModbusSerialMaster.CreateAscii(StreamResource);

        [Fact]
        public static void CreateAscii_Throws()
        {
            Assert.Throws<ArgumentNullException>("tcpClient", () => ModbusSerialMaster.CreateAscii((TcpClient)null));
            Assert.Throws<ArgumentNullException>("udpClient", () => ModbusSerialMaster.CreateAscii((UdpClient)null));
            Assert.Throws<ArgumentNullException>("streamResource", () => ModbusSerialMaster.CreateAscii((IStreamResource)null));
        }

        [Fact]
        public static void CreateAscii_TcpClient()
        {
            var client = new TcpClient();
            var master = ModbusSerialMaster.CreateAscii(client);

            Assert.NotNull(master);
            Assert.NotNull(master.Transport);
            Assert.NotNull(((IModbusSerialMaster)master).Transport);
        }

        [Fact]
        public static void CreateAscii_UdpClient()
        {
            var client = new UdpClient();

            Assert.Throws<InvalidOperationException>(() => ModbusSerialMaster.CreateAscii(client));
        }

        [Fact]
        public void CreateAscii_StreamResource()
        {
            Assert.NotNull(Master);
            Assert.NotNull(Master.Transport);
            Assert.NotNull(((IModbusSerialMaster)Master).Transport);
        }
    }
}
