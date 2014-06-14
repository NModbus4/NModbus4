using Modbus.Data;
using Modbus.Message;

namespace Modbus.UnitTests.Message
{
    using NUnit.Framework;

    [TestFixture]
	public class DiagnosticsRequestResponseFixture
	{
		[Test]
		public void ToString_Test()
		{
			var message = new DiagnosticsRequestResponse(Modbus.DiagnosticsReturnQueryData, 3, new RegisterCollection(5));

			Assert.AreEqual("Diagnostics message, sub-function return query data - {5}.", message.ToString());
		}
	}
}
