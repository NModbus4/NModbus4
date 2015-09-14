namespace Modbus.Message
{
    using System;

    /// <summary>
    ///
    /// </summary>
    public abstract class AbstractModbusMessage
    {
        private readonly ModbusMessageImpl _messageImpl;

        /// <summary>
        ///
        /// </summary>
        internal AbstractModbusMessage()
        {
            _messageImpl = new ModbusMessageImpl();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="functionCode"></param>
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
        internal ModbusMessageImpl MessageImpl
        {
            get { return _messageImpl; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="frame"></param>
        public void Initialize(byte[] frame)
        {
            if (frame.Length < MinimumFrameSize)
            {
                string msg = $"Message frame must contain at least {MinimumFrameSize} bytes of data.";
                throw new FormatException(msg);
            }

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
