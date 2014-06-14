using System;
using Modbus.Data;
using Modbus.Message;

namespace Modbus.UnitTests.Message
{
    using NUnit.Framework;

    [TestFixture]
	public class WriteMultipleCoilsRequestFixture
	{
		[Test]
		public void CreateWriteMultipleCoilsRequest()
		{
			DiscreteCollection col = new DiscreteCollection(true, false, true, false, true, true, true, false, false);
			WriteMultipleCoilsRequest request = new WriteMultipleCoilsRequest(34, 45, col);
			Assert.AreEqual(Modbus.WriteMultipleCoils, request.FunctionCode);
			Assert.AreEqual(34, request.SlaveAddress);
			Assert.AreEqual(45, request.StartAddress);
			Assert.AreEqual(9, request.NumberOfPoints);
			Assert.AreEqual(2, request.ByteCount);
			Assert.AreEqual(col.NetworkBytes, request.Data.NetworkBytes);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CreateWriteMultipleCoilsRequestTooMuchData()
		{
			new WriteMultipleCoilsRequest(1, 2, MessageUtility.CreateDefaultCollection<DiscreteCollection, bool>(true, Modbus.MaximumDiscreteRequestResponseSize + 1));
		}

		[Test]
		public void CreateWriteMultipleCoilsRequestMaxSize()
		{
			WriteMultipleCoilsRequest request = new WriteMultipleCoilsRequest(1, 2, MessageUtility.CreateDefaultCollection<DiscreteCollection, bool>(true, Modbus.MaximumDiscreteRequestResponseSize));
			Assert.AreEqual(Modbus.MaximumDiscreteRequestResponseSize, request.Data.Count);
		}

		[Test]
		public void ToString_WriteMultipleCoilsRequest()
		{
			DiscreteCollection col = new DiscreteCollection(true, false, true, false, true, true, true, false, false);
			WriteMultipleCoilsRequest request = new WriteMultipleCoilsRequest(34, 45, col);

			Assert.AreEqual("Write 9 coils starting at address 45.", request.ToString());
		}
	}
}
