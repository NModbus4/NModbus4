using System.Linq;
using Modbus.Data;

namespace Modbus.UnitTests.Data
{
    using NUnit.Framework;

    [TestFixture]
    public class DiscreteCollectionFixture
    {
        [Test]
        public void ByteCount()
        {
            DiscreteCollection col = new DiscreteCollection(true, true, false, false, false, false, false, false, false);
            Assert.AreEqual(2, col.ByteCount);
        }

        [Test]
        public void ByteCountEven()
        {
            DiscreteCollection col = new DiscreteCollection(true, true, false, false, false, false, false, false);
            Assert.AreEqual(1, col.ByteCount);
        }

        [Test]
        public void NetworkBytes()
        {
            DiscreteCollection col = new DiscreteCollection(true, true);
            Assert.AreEqual(new byte[] {3}, col.NetworkBytes);
        }

        [Test]
        public void CreateNewDiscreteCollectionInitialize()
        {
            DiscreteCollection col = new DiscreteCollection(true, true, true);
            Assert.AreEqual(3, col.Count);
            Assert.IsFalse(col.Contains(false));
        }

        [Test]
        public void CreateNewDiscreteCollectionFromBoolParams()
        {
            DiscreteCollection col = new DiscreteCollection(true, false, true);
            Assert.AreEqual(3, col.Count);
        }

        [Test]
        public void CreateNewDiscreteCollectionFromBytesParams()
        {
            DiscreteCollection col = new DiscreteCollection(1, 2, 3);
            Assert.AreEqual(24, col.Count);
        }

        [Test]
        public void CreateNewDiscreteCollectionFromBytesParamsOrder()
        {
            DiscreteCollection col = new DiscreteCollection(194);
            Assert.AreEqual(new bool[] {false, true, false, false, false, false, true, true}, col.ToArray());
        }

        [Test]
        public void CreateNewDiscreteCollectionFromBytesParamsOrder2()
        {
            DiscreteCollection col = new DiscreteCollection(157, 7);
            Assert.AreEqual(
                new bool[]
                {true, false, true, true, true, false, false, true, true, true, true, false, false, false, false, false},
                col.ToArray());
        }

        [Test]
        public void Resize()
        {
            DiscreteCollection col = new DiscreteCollection(byte.MaxValue, byte.MaxValue);
            Assert.AreEqual(16, col.Count);
            col.RemoveAt(3);
            Assert.AreEqual(15, col.Count);
        }

        [Test]
        public void BytesPersistence()
        {
            DiscreteCollection col = new DiscreteCollection(byte.MaxValue, byte.MaxValue);
            Assert.AreEqual(16, col.Count);
            byte[] originalBytes = col.NetworkBytes;
            col.RemoveAt(3);
            Assert.AreEqual(15, col.Count);
            Assert.AreNotEqual(originalBytes, col.NetworkBytes);
        }

        [Test]
        public void AddCoil()
        {
            DiscreteCollection col = new DiscreteCollection();
            Assert.AreEqual(0, col.Count);
            col.Add(true);
            Assert.AreEqual(1, col.Count);
        }
    }
}