using System;
using System.Linq;
using System.Reflection;
using Modbus.Message;

namespace Modbus.UnitTests.Message
{
    using NUnit.Framework;

    [TestFixture]
    public class ModbusMessageFixture
    {
        [Test]
        public void ProtocolDataUnitReadCoilsRequest()
        {
            ModbusMessage message = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 100, 9);
            byte[] expectedResult = {Modbus.ReadCoils, 0, 100, 0, 9};
            Assert.AreEqual(expectedResult, message.ProtocolDataUnit);
        }

        [Test]
        public void MessageFrameReadCoilsRequest()
        {
            ModbusMessage message = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 2, 3);
            byte[] expectedMessageFrame = {1, Modbus.ReadCoils, 0, 2, 0, 3};
            Assert.AreEqual(expectedMessageFrame, message.MessageFrame);
        }

        [Test]
        public void ModbusMessageToStringOverriden()
        {
            var messageTypes = from message in typeof (ModbusMessage).Assembly.GetTypes()
                where !message.IsAbstract && message.IsSubclassOf(typeof (ModbusMessage))
                select message;

            foreach (Type messageType in messageTypes)
                Assert.IsNotNull(
                    messageType.GetMethod("ToString",
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                    String.Concat("No ToString override in message ", messageType.FullName));
        }

        internal static void AssertModbusMessagePropertiesAreEqual(IModbusMessage obj1, IModbusMessage obj2)
        {
            Assert.AreEqual(obj1.FunctionCode, obj2.FunctionCode);
            Assert.AreEqual(obj1.SlaveAddress, obj2.SlaveAddress);
            Assert.AreEqual(obj1.MessageFrame, obj2.MessageFrame);
            Assert.AreEqual(obj1.ProtocolDataUnit, obj2.ProtocolDataUnit);
        }
    }
}