namespace Modbus.Message
{
    using System;
    using System.Globalization;

    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractModbusMessage
    {
        private readonly ModbusMessageImpl _messageImpl;

        internal AbstractModbusMessage()
        {
            _messageImpl = new ModbusMessageImpl();
        }

        internal AbstractModbusMessage(byte slaveAddress, byte functionCode)
        {
            _messageImpl = new ModbusMessageImpl(slaveAddress, functionCode);
        }

        /// <summary>
        /// 
        /// </summary>
        public ushort TransactionId
        {
            get { return _messageImpl.TransactionId; }
            set { _messageImpl.TransactionId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte FunctionCode
        {
            get { return _messageImpl.FunctionCode; }
            set { _messageImpl.FunctionCode = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte SlaveAddress
        {
            get { return _messageImpl.SlaveAddress; }
            set { _messageImpl.SlaveAddress = value; }
        }

        internal ModbusMessageImpl MessageImpl
        {
            get { return _messageImpl; }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] MessageFrame
        {
            get { return _messageImpl.MessageFrame; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual byte[] ProtocolDataUnit
        {
            get { return _messageImpl.ProtocolDataUnit; }
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract int MinimumFrameSize { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        public void Initialize(byte[] frame)
        {
            if (frame.Length < MinimumFrameSize)
                throw new FormatException(String.Format(CultureInfo.InvariantCulture,
                    "Message frame must contain at least {0} bytes of data.", MinimumFrameSize));

            _messageImpl.Initialize(frame);
            InitializeUnique(frame);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        protected abstract void InitializeUnique(byte[] frame);
    }
}
