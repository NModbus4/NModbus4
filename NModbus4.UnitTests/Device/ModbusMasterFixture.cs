using System;
using System.Linq;
using Modbus.Device;
using Modbus.IO;
using Rhino.Mocks;
using Xunit;

namespace Modbus.UnitTests.Device
{
    public class ModbusMasterFixture
    {
        [Fact]
        public void ReadCoils()
        {
            var mockSerialResource = MockRepository.GenerateStub<IStreamResource>();
            var master = ModbusSerialMaster.CreateRtu(mockSerialResource);

            Assert.Throws<ArgumentException>(() => master.ReadCoils(1, 1, 0));
            Assert.Throws<ArgumentException>(() => master.ReadCoils(1, 1, 2001));
        }

        [Fact]
        public void ReadInputs()
        {
            var mockSerialResource = MockRepository.GenerateStub<IStreamResource>();
            var master = ModbusSerialMaster.CreateRtu(mockSerialResource);

            Assert.Throws<ArgumentException>(() => master.ReadInputs(1, 1, 0));
            Assert.Throws<ArgumentException>(() => master.ReadInputs(1, 1, 2001));
        }

        [Fact]
        public void ReadHoldingRegisters()
        {
            var mockSerialResource = MockRepository.GenerateStub<IStreamResource>();
            var master = ModbusSerialMaster.CreateRtu(mockSerialResource);

            Assert.Throws<ArgumentException>(() => master.ReadHoldingRegisters(1, 1, 0));
            Assert.Throws<ArgumentException>(() => master.ReadHoldingRegisters(1, 1, 126));
        }

        [Fact]
        public void ReadInputRegisters()
        {
            var mockSerialResource = MockRepository.GenerateStub<IStreamResource>();
            var master = ModbusSerialMaster.CreateRtu(mockSerialResource);

            Assert.Throws<ArgumentException>(() => master.ReadInputRegisters(1, 1, 0));
            Assert.Throws<ArgumentException>(() => master.ReadInputRegisters(1, 1, 126));
        }

        [Fact]
        public void WriteMultipleRegisters()
        {
            var mockSerialResource = MockRepository.GenerateStub<IStreamResource>();
            var master = ModbusSerialMaster.CreateRtu(mockSerialResource);

            Assert.Throws<ArgumentNullException>(() => master.WriteMultipleRegisters(1, 1, null));
            Assert.Throws<ArgumentException>(() => master.WriteMultipleRegisters(1, 1, new ushort[] {}));
            Assert.Throws<ArgumentException>(
                () => master.WriteMultipleRegisters(1, 1, Enumerable.Repeat<ushort>(1, 124).ToArray()));
        }

        [Fact]
        public void WriteMultipleCoils()
        {
            var mockSerialResource = MockRepository.GenerateStub<IStreamResource>();
            var master = ModbusSerialMaster.CreateRtu(mockSerialResource);

            Assert.Throws<ArgumentNullException>(() => master.WriteMultipleCoils(1, 1, null));
            Assert.Throws<ArgumentException>(() => master.WriteMultipleCoils(1, 1, new bool[] {}));
            Assert.Throws<ArgumentException>(
                () => master.WriteMultipleCoils(1, 1, Enumerable.Repeat<bool>(false, 1969).ToArray()));
        }

        [Fact]
        public void ReadWriteMultipleRegisters()
        {
            var mockSerialResource = MockRepository.GenerateStub<IStreamResource>();
            var master = ModbusSerialMaster.CreateRtu(mockSerialResource);

            // validate numberOfPointsToRead
            Assert.Throws<ArgumentException>(() => master.ReadWriteMultipleRegisters(1, 1, 0, 1, new ushort[] {1}));
            Assert.Throws<ArgumentException>(() => master.ReadWriteMultipleRegisters(1, 1, 126, 1, new ushort[] {1}));

            // validate writeData
            Assert.Throws<ArgumentNullException>(() => master.ReadWriteMultipleRegisters(1, 1, 1, 1, null));
            Assert.Throws<ArgumentException>(() => master.ReadWriteMultipleRegisters(1, 1, 1, 1, new ushort[] {}));
            Assert.Throws<ArgumentException>(
                () => master.ReadWriteMultipleRegisters(1, 1, 1, 1, Enumerable.Repeat<ushort>(1, 122).ToArray()));
        }
    }
}