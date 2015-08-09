using System;
using Modbus.Message;
using Xunit;

namespace Modbus.UnitTests.Message
{
    public class ModbusMessageImplFixture
    {
        [Fact]
        public void ModbusMessageCtorInitializesProperties()
        {
            ModbusMessageImpl messageImpl = new ModbusMessageImpl(5, ModbusConstants.ReadCoils);
            Assert.Equal(5, messageImpl.SlaveAddress);
            Assert.Equal(ModbusConstants.ReadCoils, messageImpl.FunctionCode);
        }

        [Fact]
        public void Initialize()
        {
            ModbusMessageImpl messageImpl = new ModbusMessageImpl();
            messageImpl.Initialize(new byte[] {1, 2, 9, 9, 9, 9});
            Assert.Equal(1, messageImpl.SlaveAddress);
            Assert.Equal(2, messageImpl.FunctionCode);
        }

        [Fact]
        public void ChecckInitializeFrameNull()
        {
            ModbusMessageImpl messageImpl = new ModbusMessageImpl();
            Assert.Throws<ArgumentNullException>(() => messageImpl.Initialize(null));
        }

        [Fact]
        public void InitializeInvalidFrame()
        {
            ModbusMessageImpl messageImpl = new ModbusMessageImpl();
            Assert.Throws<FormatException>(() => messageImpl.Initialize(new byte[] {1}));
        }

        [Fact]
        public void ProtocolDataUnit()
        {
            ModbusMessageImpl messageImpl = new ModbusMessageImpl(11, ModbusConstants.ReadCoils);
            byte[] expectedResult = {ModbusConstants.ReadCoils};
            Assert.Equal(expectedResult, messageImpl.ProtocolDataUnit);
        }

        [Fact]
        public void MessageFrame()
        {
            ModbusMessageImpl messageImpl = new ModbusMessageImpl(11, ModbusConstants.ReadHoldingRegisters);
            byte[] expectedMessageFrame = {11, ModbusConstants.ReadHoldingRegisters};
            Assert.Equal(expectedMessageFrame, messageImpl.MessageFrame);
        }
    }
}