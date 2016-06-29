using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Modbus.Device;
using Modbus.IO;
using Xunit;

namespace Modbus.UnitTests.Device
{
    public class IpMasterFixture : ModbusMasterFixture
    {
        protected override ModbusMaster Master => ModbusIpMaster.CreateIp(StreamResource);

        private ModbusIpMaster IpMaster => (ModbusIpMaster)Master;

        [Fact]
        public static void CreateIp_Throws()
        {
            Assert.Throws<ArgumentNullException>("tcpClient", () => ModbusIpMaster.CreateIp((TcpClient)null));
            Assert.Throws<ArgumentNullException>("udpClient", () => ModbusIpMaster.CreateIp((UdpClient)null));
            Assert.Throws<ArgumentNullException>("streamResource", () => ModbusIpMaster.CreateIp((IStreamResource)null));
        }

        [Fact]
        public static void CreateIp_TcpClient()
        {
            var client = new TcpClient();
            var master = ModbusIpMaster.CreateIp(client);

            Assert.NotNull(master);
            Assert.NotNull(master.Transport);
        }

        [Fact]
        public static void CreateIp_UdpClient()
        {
            var client = new UdpClient();

            Assert.Throws<InvalidOperationException>(() => ModbusIpMaster.CreateIp(client));
        }

        [Fact]
        public void CreateIp_StreamResource()
        {
            Assert.NotNull(Master);
            Assert.NotNull(Master.Transport);
        }

        [Fact]
        public void ReadCoils_IpMaster()
        {
            Assert.Throws<ArgumentException>(() => IpMaster.ReadCoils(1, 0));
            Assert.Throws<ArgumentException>(() => IpMaster.ReadCoils(1, 2001));
        }

        [Fact]
        public async Task ReadCoils_IpMasterAsync()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => IpMaster.ReadCoilsAsync(1, 0)).ConfigureAwait(false);
            await Assert.ThrowsAsync<ArgumentException>(() => IpMaster.ReadCoilsAsync(1, 2001)).ConfigureAwait(false);
        }

        [Fact]
        public void ReadInputs_IpMaster()
        {
            Assert.Throws<ArgumentException>(() => IpMaster.ReadInputs(1, 0));
            Assert.Throws<ArgumentException>(() => IpMaster.ReadInputs(1, 2001));
        }

        [Fact]
        public void ReadHoldingRegisters_IpMaster()
        {
            Assert.Throws<ArgumentException>(() => IpMaster.ReadHoldingRegisters(1, 0));
            Assert.Throws<ArgumentException>(() => IpMaster.ReadHoldingRegisters(1, 126));
        }

        [Fact]
        public void ReadInputRegisters_IpMaster()
        {
            Assert.Throws<ArgumentException>(() => IpMaster.ReadInputRegisters(1, 0));
            Assert.Throws<ArgumentException>(() => IpMaster.ReadInputRegisters(1, 126));
        }

        [Fact]
        public void WriteMultipleRegisters_IpMaster()
        {
            Assert.Throws<ArgumentNullException>(() => IpMaster.WriteMultipleRegisters(1, null));
            Assert.Throws<ArgumentException>(() => IpMaster.WriteMultipleRegisters(1, new ushort[0]));
            Assert.Throws<ArgumentException>(() => IpMaster.WriteMultipleRegisters(1, Enumerable.Repeat<ushort>(1, 124).ToArray()));
        }

        [Fact]
        public void WriteMultipleCoils_IpMaster()
        {
            Assert.Throws<ArgumentNullException>(() => IpMaster.WriteMultipleCoils(1, null));
            Assert.Throws<ArgumentException>(() => IpMaster.WriteMultipleCoils(1, new bool[0]));
            Assert.Throws<ArgumentException>(() => IpMaster.WriteMultipleCoils(1, Enumerable.Repeat(false, 1969).ToArray()));
        }

        [Fact]
        public void ReadWriteMultipleRegisters_IpMaster()
        {
            // validate numberOfPointsToRead
            Assert.Throws<ArgumentException>(() => IpMaster.ReadWriteMultipleRegisters(1, 0, 1, new ushort[] { 1 }));
            Assert.Throws<ArgumentException>(() => IpMaster.ReadWriteMultipleRegisters(1, 126, 1, new ushort[] { 1 }));

            // validate writeData
            Assert.Throws<ArgumentNullException>(() => IpMaster.ReadWriteMultipleRegisters(1, 1, 1, null));
            Assert.Throws<ArgumentException>(() => IpMaster.ReadWriteMultipleRegisters(1, 1, 1, new ushort[0]));
            Assert.Throws<ArgumentException>(() => IpMaster.ReadWriteMultipleRegisters(1, 1, 1, Enumerable.Repeat<ushort>(1, 122).ToArray()));
        }

    }
}
