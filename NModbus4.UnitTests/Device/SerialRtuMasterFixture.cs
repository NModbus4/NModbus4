using System;
using System.Net.Sockets;
using Modbus.Device;
using Modbus.IO;
using Xunit;

namespace Modbus.UnitTests.Device
{
    public class SerialRtuMasterFixture : ModbusMasterFixture
    {
        protected override ModbusMaster Master => ModbusSerialMaster.CreateRtu(StreamResource);

        [Fact]
        public static void CreateRtu_Throws()
        {
            Assert.Throws<ArgumentNullException>("tcpClient", () => ModbusSerialMaster.CreateRtu((TcpClient)null));
            Assert.Throws<ArgumentNullException>("udpClient", () => ModbusSerialMaster.CreateRtu((UdpClient)null));
            Assert.Throws<ArgumentNullException>("streamResource", () => ModbusSerialMaster.CreateRtu((IStreamResource)null));
        }

        [Fact]
        public static void CreateRtu_TcpClient()
        {
            var client = new TcpClient();
            var master = ModbusSerialMaster.CreateRtu(client);

            Assert.NotNull(master);
            Assert.NotNull(master.Transport);
            Assert.NotNull(((IModbusSerialMaster)master).Transport);
        }

        [Fact]
        public static void CreateRtu_UdpClient()
        {
            var client = new UdpClient();

            Assert.Throws<InvalidOperationException>(() => ModbusSerialMaster.CreateRtu(client));
        }

        [Fact]
        public void CreateRtu_StreamResource()
        {
            Assert.NotNull(Master);
            Assert.NotNull(Master.Transport);
            Assert.NotNull(((IModbusSerialMaster)Master).Transport);
        }
    }
}
