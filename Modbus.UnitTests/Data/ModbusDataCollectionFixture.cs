using System;
using System.Linq;
using Modbus.Data;

namespace Modbus.UnitTests.Data
{
    using NUnit.Framework;

    [TestFixture]
    public class ModbusDataCollectionFixture
    {
        [Test]
        public void FromReadOnlyList()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>(new bool[] {true, false});
            Assert.AreEqual(3, col.Count);
        }

        [Test]
        public void ModbusDataCollection_FromParams()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>(true, false);
            Assert.AreEqual(3, col.Count);
        }

        [Test]
        public void Empty()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>();
            Assert.AreEqual(1, col.Count);
        }

        [Test]
        public void AddDefaultBool()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>(true, true);
            Assert.AreEqual(3, col.Count);
            Assert.AreEqual(new bool[] {false, true, true}, col.ToArray());
        }

        [Test]
        public void AddDefaultUshort()
        {
            ModbusDataCollection<ushort> col = new ModbusDataCollection<ushort>(1, 1);
            Assert.AreEqual(3, col.Count);
            Assert.AreEqual(new ushort[] {0, 1, 1}, col.ToArray());
        }

        [Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void SetZeroElementUsingItem()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>(true, false);
            col[0] = true;
        }

        [Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void InsertZeroElement()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>(true, false);
            col.Insert(0, true);
        }

        [Test]
        public void Clear()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>(true, false);
            col.Clear();
            Assert.AreEqual(1, col.Count);
        }

        [Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void RemoveAtZeroElement()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>(true, false);
            col.RemoveAt(0);
        }

        [Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void RemoveZeroElement()
        {
            ModbusDataCollection<bool> col = new ModbusDataCollection<bool>();
            col.Remove(default(bool));
        }
    }
}