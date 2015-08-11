using System;
using System.Linq;
using Modbus.Data;
using Xunit;

namespace Modbus.UnitTests.Data
{
    public class ModbusDataCollectionFixture
    {
        [Fact]
        public void FromReadOnlyList()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>(new bool[] { true, false });
            Assert.Equal(3, col.Count);
        }

        [Fact]
        public void ModbusDataCollection_FromParams()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>(true, false);
            Assert.Equal(3, col.Count);
        }

        [Fact]
        public void Empty()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>();
            Assert.Equal(1, col.Count);
        }

        [Fact]
        public void AddDefaultBool()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>(true, true);
            Assert.Equal(3, col.Count);
            Assert.Equal(new bool[] { false, true, true }, col.ToArray());
        }

        [Fact]
        public void AddDefaultUshort()
        {
            ModbusDataCollection<ushort> col = new ModbusDataCollection<ushort>(1, 1);
            Assert.Equal(3, col.Count);
            Assert.Equal(new ushort[] { 0, 1, 1 }, col.ToArray());
        }

        [Fact]
        public void SetZeroElementUsingItem()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>(true, false);
            Assert.Throws<ArgumentOutOfRangeException>(() => col[0] = true);
        }

        [Fact]
        public void InsertZeroElement()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>(true, false);
            Assert.Throws<ArgumentOutOfRangeException>(() => col.Insert(0, true));
        }

        [Fact]
        public void Clear()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>(true, false);
            col.Clear();
            Assert.Equal(1, col.Count);
        }

        [Fact]
        public void RemoveAtZeroElement()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>(true, false);
            Assert.Throws<ArgumentOutOfRangeException>(() => col.RemoveAt(0));
        }

        [Fact]
        public void RemoveZeroElement()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>();
            Assert.Throws<ArgumentOutOfRangeException>(() => col.Remove(default(bool)));
        }
    }
}