using Modbus.Data;
using Modbus.Message;

namespace Modbus.UnitTests.Message
{
    using System;

    using NUnit.Framework;

    [TestFixture]
	public class ReadHoldingInputRegistersResponseFixture
	{
		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void ReadHoldingInputRegistersResponse_NullData()
		{
			new ReadHoldingInputRegistersResponse(0, 0, null);
		}

		[Test]
		public void ReadHoldingRegistersResponse()
		{
			ReadHoldingInputRegistersResponse response = new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 5, new RegisterCollection(1, 2));
			Assert.AreEqual(Modbus.ReadHoldingRegisters, response.FunctionCode);
			Assert.AreEqual(5, response.SlaveAddress);
			Assert.AreEqual(4, response.ByteCount);
			RegisterCollection col = new RegisterCollection(1, 2);
			Assert.AreEqual(col.NetworkBytes, response.Data.NetworkBytes);
		}

		[Test]
		public void ToString_ReadHoldingRegistersResponse()
		{
			ReadHoldingInputRegistersResponse response = new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1));
			Assert.AreEqual("Read 1 holding registers.", response.ToString());
		}

		[Test]
		public void ReadInputRegistersResponse()
		{
			ReadHoldingInputRegistersResponse response = new ReadHoldingInputRegistersResponse(Modbus.ReadInputRegisters, 5, new RegisterCollection(1, 2));
			Assert.AreEqual(Modbus.ReadInputRegisters, response.FunctionCode);
			Assert.AreEqual(5, response.SlaveAddress);
			Assert.AreEqual(4, response.ByteCount);
			RegisterCollection col = new RegisterCollection(1, 2);
			Assert.AreEqual(col.NetworkBytes, response.Data.NetworkBytes);
		}

		[Test]
		public void ToString_ReadInputRegistersResponse()
		{
			ReadHoldingInputRegistersResponse response = new ReadHoldingInputRegistersResponse(Modbus.ReadInputRegisters, 1, new RegisterCollection(1));
			Assert.AreEqual("Read 1 input registers.", response.ToString());
		}
	}
}
