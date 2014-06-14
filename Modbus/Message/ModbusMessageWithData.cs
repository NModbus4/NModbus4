using Modbus.Data;

namespace Modbus.Message
{
	internal abstract class ModbusMessageWithData<TData> : ModbusMessage where TData : IModbusMessageDataCollection
	{
		public ModbusMessageWithData()
		{
		}

		public ModbusMessageWithData(byte slaveAddress, byte functionCode)
			: base(slaveAddress, functionCode)
		{
		}

		public TData Data
		{
			get { return (TData) MessageImpl.Data; }
			set { MessageImpl.Data = value; }
		}
	}
}
