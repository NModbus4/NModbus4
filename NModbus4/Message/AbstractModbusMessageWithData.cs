namespace Modbus.Message
{
    using Data;

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class AbstractModbusMessageWithData<TData> : AbstractModbusMessage where TData : IModbusMessageDataCollection
    {
        /// <summary>
        ///
        /// </summary>
        internal AbstractModbusMessageWithData()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="functionCode"></param>
        internal AbstractModbusMessageWithData(byte slaveAddress,
                                               byte functionCode)
            : base(slaveAddress, functionCode)
        {
        }

        /// <summary>
        ///
        /// </summary>
        public TData Data
        {
            get { return (TData)MessageImpl.Data; }
            set { MessageImpl.Data = value; }
        }
    }
}
