using Modbus.Message;
using Xunit;

namespace Modbus.UnitTests.Message
{
    public class SlaveExceptionResponseFixture
    {
        [Fact]
        public void CreateSlaveExceptionResponse()
        {
            SlaveExceptionResponse response = new SlaveExceptionResponse(11, ModbusConstants.ReadCoils + ModbusConstants.ExceptionOffset,
                2);
            Assert.Equal(11, response.SlaveAddress);
            Assert.Equal(ModbusConstants.ReadCoils + ModbusConstants.ExceptionOffset, response.FunctionCode);
            Assert.Equal(2, response.SlaveExceptionCode);
        }

        [Fact]
        public void SlaveExceptionResponsePDU()
        {
            SlaveExceptionResponse response = new SlaveExceptionResponse(11, ModbusConstants.ReadCoils + ModbusConstants.ExceptionOffset,
                2);
            Assert.Equal(new byte[] {response.FunctionCode, response.SlaveExceptionCode}, response.ProtocolDataUnit);
        }
    }
}