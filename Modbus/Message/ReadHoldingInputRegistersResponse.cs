using System;
using System.Globalization;
using System.Linq;
using Modbus.Data;

namespace Modbus.Message
{
    using Unme.Common;

    internal class ReadHoldingInputRegistersResponse : ModbusMessageWithData<RegisterCollection>, IModbusMessage
	{
		public ReadHoldingInputRegistersResponse()
		{
		}

		public ReadHoldingInputRegistersResponse(byte functionCode, byte slaveAddress, RegisterCollection data)
			: base(slaveAddress, functionCode)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			ByteCount = data.ByteCount;
			Data = data;
		}

		public byte ByteCount
		{
			get { return MessageImpl.ByteCount.Value; }
			set { MessageImpl.ByteCount = value; }
		}

		public override int MinimumFrameSize
		{
			get { return 3; }
		}

		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Read {0} {1} registers.", Data.Count, FunctionCode == Modbus.ReadHoldingRegisters ? "holding" : "input");
		}

		protected override void InitializeUnique(byte[] frame)
		{
			if (frame.Length < MinimumFrameSize + frame[2])
				throw new FormatException("Message frame does not contain enough bytes.");

			ByteCount = frame[2];
			Data = new RegisterCollection(frame.Slice(3, ByteCount).ToArray());
		}
	}
}
