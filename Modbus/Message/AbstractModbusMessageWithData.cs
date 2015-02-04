using Modbus.Data;

namespace Modbus.Message
{
    internal abstract class AbstractModbusMessageWithData<TData> : AbstractModbusMessage where TData : IModbusMessageDataCollection
    {
        public AbstractModbusMessageWithData()
        {
        }

        public AbstractModbusMessageWithData(byte slaveAddress, byte functionCode)
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