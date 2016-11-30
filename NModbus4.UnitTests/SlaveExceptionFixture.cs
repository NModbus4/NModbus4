using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;
using Modbus.Message;

namespace Modbus.UnitTests
{
    public class SlaveExceptionFixture
    {
        [Fact]
        public void CreateSlaveException_EmptyConstructor()
        {
            SlaveException se = new SlaveException();
            Assert.Equal("Exception of type 'Modbus.SlaveException' was thrown.", se.Message);
        }

        [Fact]
        public void CreateSlaveException_Message()
        {
            SlaveException se = new SlaveException("Hello World");
            Assert.Equal("Hello World", se.Message);
        }

        [Fact]
        public void CreateSlaveException_MessageAndInnerException()
        {
            SlaveException se = new SlaveException("Foo", new IOException("Bar"));
            Assert.Equal("Foo", se.Message);
            Assert.NotNull(se.InnerException);
            Assert.Equal("Bar", se.InnerException.Message);
        }

        [Fact]
        public void CreateSlaveException_SlaveExceptionResponse()
        {
            SlaveExceptionResponse response = new SlaveExceptionResponse(12, ModbusConstants.ReadCoils, 1);
            SlaveException se = new SlaveException(response);
            Assert.Equal(
                String.Format(
                    "Exception of type 'Modbus.SlaveException' was thrown.\r\nFunction Code: {0}\r\nException Code: {1} - {2}",
                    response.FunctionCode, response.SlaveExceptionCode, Resources.IllegalFunction), se.Message);
        }

        [Fact]
        public void CreateSlaveException_CustomMessageAndSlaveExceptionResponse()
        {
            SlaveExceptionResponse response = new SlaveExceptionResponse(12, ModbusConstants.ReadCoils, 2);
            string customMessage = "custom message";
            SlaveException se = new SlaveException(customMessage, response);
            Assert.Equal(String.Format("{0}\r\nFunction Code: {1}\r\nException Code: {2} - {3}",
                customMessage, response.FunctionCode, response.SlaveExceptionCode, Resources.IllegalDataAddress),
                se.Message);
        }

        [Fact]
        public void FunctionCode_SlaveExceptionInitialized()
        {
            SlaveException se = new SlaveException(new SlaveExceptionResponse(1, 2, 3));
            Assert.Equal(2, se.FunctionCode);
        }

        [Fact]
        public void FunctionCode_SlaveExceptionNotInitialized()
        {
            SlaveException se = new SlaveException();
            Assert.Equal(0, se.FunctionCode);
        }

        [Fact]
        public void SlaveException_SlaveExceptionInitialized()
        {
            SlaveException se = new SlaveException(new SlaveExceptionResponse(1, 2, 3));
            Assert.Equal(3, se.SlaveExceptionCode);
        }

        [Fact]
        public void SlaveExceptionCode_SlaveExceptionNotInitialized()
        {
            SlaveException se = new SlaveException();
            Assert.Equal(0, se.SlaveExceptionCode);
        }

        [Fact]
        public void SlaveAddress_SlaveExceptionInitialized()
        {
            SlaveException se = new SlaveException(new SlaveExceptionResponse(1, 2, 3));
            Assert.Equal(1, se.SlaveAddress);
        }

        [Fact]
        public void SlaveAddress_SlaveExceptionNotInitialized()
        {
            SlaveException se = new SlaveException();
            Assert.Equal(0, se.SlaveAddress);
        }

        [Fact]
        public void SlaveException_Serializable()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            SlaveException slaveException = new SlaveException(new SlaveExceptionResponse(1, 2, 3));

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, slaveException);
                stream.Position = 0;

                SlaveException slaveException2 = formatter.Deserialize(stream) as SlaveException;
                Assert.NotNull(slaveException2);
                Assert.Equal(1, slaveException2.SlaveAddress);
                Assert.Equal(2, slaveException2.FunctionCode);
                Assert.Equal(3, slaveException2.SlaveExceptionCode);
            }
        }
    }
}