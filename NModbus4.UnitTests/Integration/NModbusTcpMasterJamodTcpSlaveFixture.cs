using System.Net.Sockets;
using Modbus.Device;

namespace Modbus.IntegrationTests
{
    public class NModbusTcpMasterJamodTcpSlaveFixture : ModbusMasterFixture
    {
        public NModbusTcpMasterJamodTcpSlaveFixture()
        {
            string program = $"TcpSlave {Port}";
            StartJamodSlave(program);

            MasterTcp = new TcpClient(AddressFamily.InterNetwork);
            MasterTcp.ConnectAsync(TcpHost.ToString(), Port).GetAwaiter().GetResult();
            Master = ModbusIpMaster.CreateIp(MasterTcp);
        }

        /// <summary>
        /// Not supported by the Jamod Slave
        /// </summary>
        public override void ReadWriteMultipleRegisters()
        {
        }
    }
}
