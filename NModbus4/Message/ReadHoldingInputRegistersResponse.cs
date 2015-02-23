namespace Modbus.Message
{
    using System;
    using System.Globalization;
    using System.Linq;

    using Data;

    using Unme.Common;

    /// <summary>
    /// 
    /// </summary>
    public class ReadHoldingInputRegistersResponse : AbstractModbusMessageWithData<RegisterCollection>, IModbusMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public ReadHoldingInputRegistersResponse()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="functionCode"></param>
        /// <param name="slaveAddress"></param>
        /// <param name="data"></param>
        public ReadHoldingInputRegistersResponse(byte functionCode, byte slaveAddress, RegisterCollection data)
            : base(slaveAddress, functionCode)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            ByteCount = data.ByteCount;
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        public byte ByteCount
        {
            get { return MessageImpl.ByteCount.Value; }
            set { MessageImpl.ByteCount = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int MinimumFrameSize
        {
            get { return 3; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "Read {0} {1} registers.", Data.Count,
                FunctionCode == Modbus.ReadHoldingRegisters ? "holding" : "input");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        protected override void InitializeUnique(byte[] frame)
        {
            if (frame.Length < MinimumFrameSize + frame[2])
                throw new FormatException("Message frame does not contain enough bytes.");

            ByteCount = frame[2];
            Data = new RegisterCollection(frame.Slice(3, ByteCount).ToArray());
        }
    }
}
