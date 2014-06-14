using Modbus.Data;
using Modbus.Message;

namespace Modbus.UnitTests.Message
{
    using NUnit.Framework;

    [TestFixture]
	public class ReadCoilsResponseFixture
	{
		[Test]
		public void CreateReadCoilsResponse()
		{
			ReadCoilsInputsResponse response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 5, 2, new DiscreteCollection(true, true, true, true, true, true, false, false, true, true, false));
			Assert.AreEqual(Modbus.ReadCoils, response.FunctionCode);
			Assert.AreEqual(5, response.SlaveAddress);
			Assert.AreEqual(2, response.ByteCount);
			DiscreteCollection col = new DiscreteCollection(true, true, true, true, true, true, false, false, true, true, false);
			Assert.AreEqual(col.NetworkBytes, response.Data.NetworkBytes);
		}

		[Test]
		public void CreateReadInputsResponse()
		{
			ReadCoilsInputsResponse response = new ReadCoilsInputsResponse(Modbus.ReadInputs, 5, 2, new DiscreteCollection(true, true, true, true, true, true, false, false, true, true, false));
			Assert.AreEqual(Modbus.ReadInputs, response.FunctionCode);
			Assert.AreEqual(5, response.SlaveAddress);
			Assert.AreEqual(2, response.ByteCount);
			DiscreteCollection col = new DiscreteCollection(true, true, true, true, true, true, false, false, true, true, false);
			Assert.AreEqual(col.NetworkBytes, response.Data.NetworkBytes);
		}

		[Test]
		public void ToString_Coils()
		{
			ReadCoilsInputsResponse response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 5, 2, new DiscreteCollection(true, true, true, true, true, true, false, false, true, true, false));

			Assert.AreEqual("Read 11 coils - {1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0}.", response.ToString());
		}

		[Test]
		public void ToString_Inputs()
		{
			ReadCoilsInputsResponse response = new ReadCoilsInputsResponse(Modbus.ReadInputs, 5, 2, new DiscreteCollection(true, true, true, true, true, true, false, false, true, true, false));

			Assert.AreEqual("Read 11 inputs - {1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0}.", response.ToString());
		}
	}
}
