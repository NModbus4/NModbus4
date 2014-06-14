using System;
using Modbus.Message;

namespace Modbus.UnitTests.Message
{
    using NUnit.Framework;

    [TestFixture]
	public class ReadCoilsInputsRequestFixture
	{
		[Test]
		public void CreateReadCoilsRequest()
		{
			ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 5, 1, 10);
			Assert.AreEqual(Modbus.ReadCoils, request.FunctionCode);
			Assert.AreEqual(5, request.SlaveAddress);
			Assert.AreEqual(1, request.StartAddress);
			Assert.AreEqual(10, request.NumberOfPoints);
		}

		[Test]
		public void CreateReadInputsRequest()
		{
			ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadInputs, 5, 1, 10);
			Assert.AreEqual(Modbus.ReadInputs, request.FunctionCode);
			Assert.AreEqual(5, request.SlaveAddress);
			Assert.AreEqual(1, request.StartAddress);
			Assert.AreEqual(10, request.NumberOfPoints);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CreateReadCoilsInputsRequestTooMuchData()
		{
			new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 2, Modbus.MaximumDiscreteRequestResponseSize + 1);
		}

		[Test]
		public void CreateReadCoilsInputsRequestMaxSize()
		{
			ReadCoilsInputsRequest response = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 2, Modbus.MaximumDiscreteRequestResponseSize);
			Assert.AreEqual(Modbus.MaximumDiscreteRequestResponseSize, response.NumberOfPoints);
		}

		[Test]
		public void ToString_ReadCoilsRequest()
		{
			ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 5, 1, 10);

			Assert.AreEqual("Read 10 coils starting at address 1.", request.ToString());
		}

		[Test]
		public void ToString_ReadInputsRequest()
		{
			ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadInputs, 5, 1, 10);

			Assert.AreEqual("Read 10 inputs starting at address 1.", request.ToString());
		}
	}
}
