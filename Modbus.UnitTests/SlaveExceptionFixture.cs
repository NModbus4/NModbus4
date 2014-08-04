using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Modbus.Message;

namespace Modbus.UnitTests
{
    [TestFixture]
    public class SlaveExceptionFixture
    {
        [Test]
        public void CreateSlaveException_EmptyConstructor()
        {
            SlaveException se = new SlaveException();
            Assert.AreEqual("Exception of type 'Modbus.SlaveException' was thrown.", se.Message);
        }

        [Test]
        public void CreateSlaveException_Message()
        {
            SlaveException se = new SlaveException("Hello World");
            Assert.AreEqual("Hello World", se.Message);
        }

        [Test]
        public void CreateSlaveException_MessageAndInnerException()
        {
            SlaveException se = new SlaveException("Foo", new IOException("Bar"));
            Assert.AreEqual("Foo", se.Message);
            Assert.IsNotNull(se.InnerException);
            Assert.AreEqual("Bar", se.InnerException.Message);
        }

        [Test]
        public void CreateSlaveException_SlaveExceptionResponse()
        {
            SlaveExceptionResponse response = new SlaveExceptionResponse(12, Modbus.ReadCoils, 1);
            SlaveException se = new SlaveException(response);
            Assert.AreEqual(
                String.Format(
                    "Exception of type 'Modbus.SlaveException' was thrown.\r\nFunction Code: {0}\r\nException Code: {1} - {2}",
                    response.FunctionCode, response.SlaveExceptionCode, Resources.IllegalFunction), se.Message);
        }

        [Test]
        public void CreateSlaveException_CustomMessageAndSlaveExceptionResponse()
        {
            SlaveExceptionResponse response = new SlaveExceptionResponse(12, Modbus.ReadCoils, 2);
            string customMessage = "custom message";
            SlaveException se = new SlaveException(customMessage, response);
            Assert.AreEqual(String.Format("{0}\r\nFunction Code: {1}\r\nException Code: {2} - {3}",
                customMessage, response.FunctionCode, response.SlaveExceptionCode, Resources.IllegalDataAddress),
                se.Message);
        }

        [Test]
        public void FunctionCode_SlaveExceptionInitialized()
        {
            SlaveException se = new SlaveException(new SlaveExceptionResponse(1, 2, 3));
            Assert.AreEqual(2, se.FunctionCode);
        }

        [Test]
        public void FunctionCode_SlaveExceptionNotInitialized()
        {
            SlaveException se = new SlaveException();
            Assert.AreEqual(0, se.FunctionCode);
        }

        [Test]
        public void SlaveException_SlaveExceptionInitialized()
        {
            SlaveException se = new SlaveException(new SlaveExceptionResponse(1, 2, 3));
            Assert.AreEqual(3, se.SlaveExceptionCode);
        }

        [Test]
        public void SlaveExceptionCode_SlaveExceptionNotInitialized()
        {
            SlaveException se = new SlaveException();
            Assert.AreEqual(0, se.SlaveExceptionCode);
        }

        [Test]
        public void SlaveAddress_SlaveExceptionInitialized()
        {
            SlaveException se = new SlaveException(new SlaveExceptionResponse(1, 2, 3));
            Assert.AreEqual(1, se.SlaveAddress);
        }

        [Test]
        public void SlaveAddress_SlaveExceptionNotInitialized()
        {
            SlaveException se = new SlaveException();
            Assert.AreEqual(0, se.SlaveAddress);
        }

        [Test]
        public void SlaveException_Serializable()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            SlaveException slaveException = new SlaveException(new SlaveExceptionResponse(1, 2, 3));

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, slaveException);
                stream.Position = 0;

                SlaveException slaveException2 = formatter.Deserialize(stream) as SlaveException;
                Assert.IsNotNull(slaveException2);
                Assert.AreEqual(1, slaveException2.SlaveAddress);
                Assert.AreEqual(2, slaveException2.FunctionCode);
                Assert.AreEqual(3, slaveException2.SlaveExceptionCode);
            }
        }
    }
}