using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Modbus.Device;
using Xunit;

namespace Modbus.IntegrationTests
{
    public class ModbusIpMasterFixture
    {
        [Fact]
        public async Task OverrideTimeoutOnTcpClient()
        {
            var listener = new TcpListener(ModbusMasterFixture.TcpHost, ModbusMasterFixture.Port);
            Task slaveTask;
            using (var slave = ModbusTcpSlave.CreateTcp(ModbusMasterFixture.SlaveAddress, listener))
            {
                slaveTask = Task.Run((Action)slave.Listen);

                var client = new TcpClient(ModbusMasterFixture.TcpHost.ToString(), ModbusMasterFixture.Port);
                client.ReceiveTimeout = 1500;
                client.SendTimeout = 3000;
                using (var master = ModbusIpMaster.CreateIp(client))
                {
                    Assert.Equal(1500, client.GetStream().ReadTimeout);
                    Assert.Equal(3000, client.GetStream().WriteTimeout);
                }
            }

            await slaveTask;
        }

        [Fact]
        public async Task OverrideTimeoutOnNetworkStream()
        {
            var listener = new TcpListener(ModbusMasterFixture.TcpHost, ModbusMasterFixture.Port);
            Task slaveTask;
            using (var slave = ModbusTcpSlave.CreateTcp(ModbusMasterFixture.SlaveAddress, listener))
            {
                slaveTask = Task.Run((Action)slave.Listen);

                var client = new TcpClient(ModbusMasterFixture.TcpHost.ToString(), ModbusMasterFixture.Port);
                client.GetStream().ReadTimeout = 1500;
                client.GetStream().WriteTimeout = 3000;
                using (var master = ModbusIpMaster.CreateIp(client))
                {
                    Assert.Equal(1500, client.GetStream().ReadTimeout);
                    Assert.Equal(3000, client.GetStream().WriteTimeout);
                }
            }

            await slaveTask;
        }
    }
}
