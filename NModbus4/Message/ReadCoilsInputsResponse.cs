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
    public class ReadCoilsInputsResponse : AbstractModbusMessageWithData<DiscreteCollection>, IModbusMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public ReadCoilsInputsResponse()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="functionCode"></param>
        /// <param name="slaveAddress"></param>
        /// <param name="byteCount"></param>
        /// <param name="data"></param>
        public ReadCoilsInputsResponse(byte functionCode, byte slaveAddress, byte byteCount, DiscreteCollection data)
            : base(slaveAddress, functionCode)
        {
            ByteCount = byteCount;
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
            return String.Format(CultureInfo.InvariantCulture,
                "Read {0} {1} - {2}.",
                Data.Count(),
                FunctionCode == Modbus.ReadInputs ? "inputs" : "coils",
                Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        protected override void InitializeUnique(byte[] frame)
        {
            if (frame.Length < 3 + frame[2])
                throw new FormatException("Message frame data segment does not contain enough bytes.");

            ByteCount = frame[2];
            Data = new DiscreteCollection(frame.Slice(3, ByteCount).ToArray());
        }
    }
}
