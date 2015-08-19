using System.Net.Sockets;
using Modbus.Device;

namespace Modbus.IntegrationTests
{
    public class NModbusUdpMasterNModbusUdpSlaveFixture : ModbusMasterFixture
    {
        public NModbusUdpMasterNModbusUdpSlaveFixture()
        {
            SlaveUdp = new UdpClient(Port);
            Slave = ModbusUdpSlave.CreateUdp(SlaveUdp);
            StartSlave();

            MasterUdp = new UdpClient();
            MasterUdp.Connect(DefaultModbusIPEndPoint);
            Master = ModbusIpMaster.CreateIp(MasterUdp);
        }
    }
}
