using Modbus.Message;

namespace Modbus.UnitTests.Message
{
    using NUnit.Framework;

    [TestFixture]
    public class SlaveExceptionResponseFixture
    {
        [Test]
        public void CreateSlaveExceptionResponse()
        {
            SlaveExceptionResponse response = new SlaveExceptionResponse(11, Modbus.ReadCoils + Modbus.ExceptionOffset,
                2);
            Assert.AreEqual(11, response.SlaveAddress);
            Assert.AreEqual(Modbus.ReadCoils + Modbus.ExceptionOffset, response.FunctionCode);
            Assert.AreEqual(2, response.SlaveExceptionCode);
        }

        [Test]
        public void SlaveExceptionResponsePDU()
        {
            SlaveExceptionResponse response = new SlaveExceptionResponse(11, Modbus.ReadCoils + Modbus.ExceptionOffset,
                2);
            Assert.AreEqual(new byte[] {response.FunctionCode, response.SlaveExceptionCode}, response.ProtocolDataUnit);
        }
    }
}