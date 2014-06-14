using Modbus.Data;
using Modbus.Message;

namespace Modbus.UnitTests.Message
{
    using NUnit.Framework;

    [TestFixture]
	public class ReadWriteMultipleRegistersRequestFixture
	{
		[Test]
		public void ReadWriteMultipleRegistersRequest()
		{
			RegisterCollection writeCollection = new RegisterCollection(255, 255, 255);
			ReadWriteMultipleRegistersRequest request = new ReadWriteMultipleRegistersRequest(5, 3, 6, 14, writeCollection);
			Assert.AreEqual(Modbus.ReadWriteMultipleRegisters, request.FunctionCode);
			Assert.AreEqual(5, request.SlaveAddress);

			// test read
			Assert.IsNotNull(request.ReadRequest);
			Assert.AreEqual(request.SlaveAddress, request.ReadRequest.SlaveAddress);
			Assert.AreEqual(3, request.ReadRequest.StartAddress);
			Assert.AreEqual(6, request.ReadRequest.NumberOfPoints);

			// test write
			Assert.IsNotNull(request.WriteRequest);
			Assert.AreEqual(request.SlaveAddress, request.WriteRequest.SlaveAddress);
			Assert.AreEqual(14, request.WriteRequest.StartAddress);
			Assert.AreEqual(writeCollection.NetworkBytes, request.WriteRequest.Data.NetworkBytes);
		}

		[Test]
		public void ProtocolDataUnit()
		{
			RegisterCollection writeCollection = new RegisterCollection(255, 255, 255);
			ReadWriteMultipleRegistersRequest request = new ReadWriteMultipleRegistersRequest(5, 3, 6, 14, writeCollection);
			byte[] pdu = { 0x17, 0x00, 0x03, 0x00, 0x06, 0x00, 0x0e, 0x00, 0x03, 0x06, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff };
			Assert.AreEqual(pdu, request.ProtocolDataUnit);
		}

		[Test]
		public void ToString_ReadWriteMultipleRegistersRequest()
		{
			RegisterCollection writeCollection = new RegisterCollection(255, 255, 255);
			ReadWriteMultipleRegistersRequest request = new ReadWriteMultipleRegistersRequest(5, 3, 6, 14, writeCollection);

			Assert.AreEqual("Write 3 holding registers starting at address 14, and read 6 registers starting at address 3.", request.ToString());
		}
	}
}
