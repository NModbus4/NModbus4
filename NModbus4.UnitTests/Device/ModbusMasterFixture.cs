using System;
using System.Linq;
using System.Threading.Tasks;
using Modbus.Device;
using Modbus.IO;
using Moq;
using Xunit;

namespace Modbus.UnitTests.Device
{
    public abstract class ModbusMasterFixture
    {
        protected static IStreamResource StreamResource => new Mock<IStreamResource>(MockBehavior.Strict).Object;

        protected abstract ModbusMaster Master { get; }

        [Fact]
        public void ReadCoils()
        {
            Assert.Throws<ArgumentException>(() => Master.ReadCoils(1, 1, 0));
            Assert.Throws<ArgumentException>(() => Master.ReadCoils(1, 1, 2001));
        }

        [Fact]
        public void ReadInputs()
        {
            Assert.Throws<ArgumentException>(() => Master.ReadInputs(1, 1, 0));
            Assert.Throws<ArgumentException>(() => Master.ReadInputs(1, 1, 2001));
        }

        [Fact]
        public async Task ReadInputsAsync()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => Master.ReadInputsAsync(1, 1, 0)).ConfigureAwait(false);
            await Assert.ThrowsAsync<ArgumentException>(() => Master.ReadInputsAsync(1, 1, 2001)).ConfigureAwait(false);
        }

        [Fact]
        public void ReadHoldingRegisters()
        {
            Assert.Throws<ArgumentException>(() => Master.ReadHoldingRegisters(1, 1, 0));
            Assert.Throws<ArgumentException>(() => Master.ReadHoldingRegisters(1, 1, 126));
        }

        [Fact]
        public async Task ReadHoldingRegistersAsync()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => Master.ReadHoldingRegistersAsync(1, 1, 0)).ConfigureAwait(false);
            await Assert.ThrowsAsync<ArgumentException>(() => Master.ReadHoldingRegistersAsync(1, 1, 126)).ConfigureAwait(false);
        }

        [Fact]
        public void ReadInputRegisters()
        {
            Assert.Throws<ArgumentException>(() => Master.ReadInputRegisters(1, 1, 0));
            Assert.Throws<ArgumentException>(() => Master.ReadInputRegisters(1, 1, 126));
        }

        [Fact]
        public async Task ReadInputRegistersAsync()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => Master.ReadInputRegistersAsync(1, 1, 0)).ConfigureAwait(false);
            await Assert.ThrowsAsync<ArgumentException>(() => Master.ReadInputRegistersAsync(1, 1, 126)).ConfigureAwait(false);
        }

        [Fact]
        public void WriteMultipleRegisters()
        {
            Assert.Throws<ArgumentNullException>(() => Master.WriteMultipleRegisters(1, 1, null));
            Assert.Throws<ArgumentException>(() => Master.WriteMultipleRegisters(1, 1, new ushort[0]));
            Assert.Throws<ArgumentException>(() => Master.WriteMultipleRegisters(1, 1, Enumerable.Repeat<ushort>(1, 124).ToArray()));
        }

        [Fact]
        public async Task WriteMultipleRegistersAsync()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => Master.WriteMultipleRegistersAsync(1, 1, null)).ConfigureAwait(false);
            await Assert.ThrowsAsync<ArgumentException>(() => Master.WriteMultipleRegistersAsync(1, 1, new ushort[0])).ConfigureAwait(false);
            await Assert.ThrowsAsync<ArgumentException>(() => Master.WriteMultipleRegistersAsync(1, 1, Enumerable.Repeat<ushort>(1, 124).ToArray())).ConfigureAwait(false);
        }

        [Fact]
        public void WriteMultipleCoils()
        {
            Assert.Throws<ArgumentNullException>(() => Master.WriteMultipleCoils(1, 1, null));
            Assert.Throws<ArgumentException>(() => Master.WriteMultipleCoils(1, 1, new bool[0]));
            Assert.Throws<ArgumentException>(() => Master.WriteMultipleCoils(1, 1, Enumerable.Repeat(false, 1969).ToArray()));
        }

        [Fact]
        public async Task WriteMultipleCoilsAsync()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => Master.WriteMultipleCoilsAsync(1, 1, null)).ConfigureAwait(false);
            await Assert.ThrowsAsync<ArgumentException>(() => Master.WriteMultipleCoilsAsync(1, 1, new bool[0])).ConfigureAwait(false);
            await Assert.ThrowsAsync<ArgumentException>(() => Master.WriteMultipleCoilsAsync(1, 1, Enumerable.Repeat(false, 1969).ToArray())).ConfigureAwait(false);
        }

        [Fact]
        public void ReadWriteMultipleRegisters()
        {
            // validate numberOfPointsToRead
            Assert.Throws<ArgumentException>(() => Master.ReadWriteMultipleRegisters(1, 1, 0, 1, new ushort[] { 1 }));
            Assert.Throws<ArgumentException>(() => Master.ReadWriteMultipleRegisters(1, 1, 126, 1, new ushort[] { 1 }));

            // validate writeData
            Assert.Throws<ArgumentNullException>(() => Master.ReadWriteMultipleRegisters(1, 1, 1, 1, null));
            Assert.Throws<ArgumentException>(() => Master.ReadWriteMultipleRegisters(1, 1, 1, 1, new ushort[0]));
            Assert.Throws<ArgumentException>(() => Master.ReadWriteMultipleRegisters(1, 1, 1, 1, Enumerable.Repeat<ushort>(1, 122).ToArray()));
        }

        [Fact]
        public async Task ReadWriteMultipleRegistersAsync()
        {
            // validate numberOfPointsToRead
            await Assert.ThrowsAsync<ArgumentException>(() => Master.ReadWriteMultipleRegistersAsync(1, 1, 0, 1, new ushort[] { 1 })).ConfigureAwait(false);
            await Assert.ThrowsAsync<ArgumentException>(() => Master.ReadWriteMultipleRegistersAsync(1, 1, 126, 1, new ushort[] { 1 })).ConfigureAwait(false);

            // validate writeData
            await Assert.ThrowsAsync<ArgumentNullException>(() => Master.ReadWriteMultipleRegistersAsync(1, 1, 1, 1, null)).ConfigureAwait(false);
            await Assert.ThrowsAsync<ArgumentException>(() => Master.ReadWriteMultipleRegistersAsync(1, 1, 1, 1, new ushort[0])).ConfigureAwait(false);
            await Assert.ThrowsAsync<ArgumentException>(() => Master.ReadWriteMultipleRegistersAsync(1, 1, 1, 1, Enumerable.Repeat<ushort>(1, 122).ToArray())).ConfigureAwait(false);
        }
    }
}
