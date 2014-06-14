using Modbus.Message;

namespace Modbus.UnitTests.Message
{
    using NUnit.Framework;

    [TestFixture]
	public class WriteSingleRegisterRequestResponseFixture
	{
		[Test]
		public void NewWriteSingleRegisterRequestResponse()
		{
			WriteSingleRegisterRequestResponse message = new WriteSingleRegisterRequestResponse(12, 5, 1200);
			Assert.AreEqual(12, message.SlaveAddress);
			Assert.AreEqual(5, message.StartAddress);
			Assert.AreEqual(1, message.Data.Count);
			Assert.AreEqual(1200, message.Data[0]);
		}

		[Test]
		public void ToStringOverride()
		{
			WriteSingleRegisterRequestResponse message = new WriteSingleRegisterRequestResponse(12, 5, 1200);
			Assert.AreEqual("Write single holding register 1200 at address 5.", message.ToString());
		}
	}
}
