using System.Collections.Generic;
using System.Linq;
using Modbus.Data;
using Xunit;

namespace Modbus.UnitTests.Data
{
    public class DataStoreFixture
    {
        [Fact]
        public void ReadData()
        {
            DataStore dataStore = new DataStore();
            ModbusDataCollection<ushort> slaveCol = new ModbusDataCollection<ushort>(0, 1, 2, 3, 4, 5, 6);
            RegisterCollection result = dataStore.ReadData(slaveCol, 1, 3);
            Assert.Equal(new ushort[] { 1, 2, 3 }, result.ToArray());
        }

        [Fact]
        public void ReadDataStartAddressTooLarge()
        {
            DataStore dataStore = new DataStore();
            Assert.Throws<InvalidModbusRequestException>(() =>
                dataStore.ReadData(new ModbusDataCollection<bool>(), 3, 2));
        }

        [Fact]
        public void ReadDataCountTooLarge()
        {
            DataStore dataStore = new DataStore();
            Assert.Throws<InvalidModbusRequestException>(() => dataStore.ReadData(
                new ModbusDataCollection<bool>(true, false, true, true), 1, 5));
        }

        [Fact]
        public void ReadDataStartAddressZero()
        {
            DataStore dataStore = new DataStore();
            dataStore.ReadData(
                new ModbusDataCollection<bool>(true, false, true, true, true, true), 0, 5);
        }

        [Fact]
        public void WriteDataSingle()
        {
            DataStore dataStore = new DataStore();
            ModbusDataCollection<bool> destination = new ModbusDataCollection<bool>(true, true);
            DiscreteCollection newValues = new DiscreteCollection(false);
            dataStore.WriteData(newValues, destination, 0);
            Assert.Equal(false, destination[1]);
        }

        [Fact]
        public void WriteDataMultiple()
        {
            DataStore dataStore = new DataStore();
            ModbusDataCollection<bool> destination = new ModbusDataCollection<bool>(false, false, false, false, false,
                false, true);
            DiscreteCollection newValues = new DiscreteCollection(true, true, true, true);
            dataStore.WriteData(newValues, destination, 0);
            Assert.Equal(new bool[] { false, true, true, true, true, false, false, true }, destination.ToArray());
        }

        [Fact]
        public void WriteDataTooLarge()
        {
            DataStore dataStore = new DataStore();
            ModbusDataCollection<bool> slaveCol = new ModbusDataCollection<bool>(true);
            DiscreteCollection newValues = new DiscreteCollection(false, false);
            Assert.Throws<InvalidModbusRequestException>(() => dataStore.WriteData(newValues, slaveCol, 1));
        }

        [Fact]
        public void WriteDataStartAddressZero()
        {
            DataStore dataStore = new DataStore();
            dataStore.WriteData(new DiscreteCollection(false),
                new ModbusDataCollection<bool>(true, true), 0);
        }

        [Fact]
        public void WriteDataStartAddressTooLarge()
        {
            DataStore dataStore = new DataStore();
            Assert.Throws<InvalidModbusRequestException>(() => dataStore.WriteData( new DiscreteCollection(true), new ModbusDataCollection<bool>(true), 2));
        }

        /// <summary>
        ///     http://modbus.org/docs/Modbus_Application_Protocol_V1_1b.pdf
        ///     In the PDU Coils are addressed starting at zero. Therefore coils numbered 1-16 are addressed as 0-15.
        ///     So reading Modbus address 0 should get you array index 1 in the DataStore.
        ///     This implies that the DataStore array index 0 can never be used.
        /// </summary>
        [Fact]
        public void TestReadMapping()
        {
            DataStore dataStore = DataStoreFactory.CreateDefaultDataStore();
            dataStore.HoldingRegisters.Insert(1, 45);
            dataStore.HoldingRegisters.Insert(2, 42);

            Assert.Equal(45,
                dataStore.ReadData(dataStore.HoldingRegisters, 0, 1)[0]);
            Assert.Equal(42,
                dataStore.ReadData(dataStore.HoldingRegisters, 1, 1)[0]);
        }

        [Fact]
        public void DataStoreReadFromEvent_ReadHoldingRegisters()
        {
            DataStore dataStore = DataStoreFactory.CreateTestDataStore();

            bool readFromEventFired = false;
            bool writtenToEventFired = false;

            dataStore.DataStoreReadFrom += (obj, e) =>
            {
                readFromEventFired = true;
                Assert.Equal(3, e.StartAddress);
                Assert.Equal(new ushort[] { 4, 5, 6 }, e.Data.B.ToArray());
                Assert.Equal(ModbusDataType.HoldingRegister, e.ModbusDataType);
            };

            dataStore.DataStoreWrittenTo += (obj, e) => writtenToEventFired = true;

            dataStore.ReadData(dataStore.HoldingRegisters, 3, 3);
            Assert.True(readFromEventFired);
            Assert.False(writtenToEventFired);
        }

        [Fact]
        public void DataStoreReadFromEvent_ReadInputRegisters()
        {
            DataStore dataStore = DataStoreFactory.CreateTestDataStore();

            bool readFromEventFired = false;
            bool writtenToEventFired = false;

            dataStore.DataStoreReadFrom += (obj, e) =>
            {
                readFromEventFired = true;
                Assert.Equal(4, e.StartAddress);
                Assert.Equal(new ushort[] { }, e.Data.B.ToArray());
                Assert.Equal(ModbusDataType.InputRegister, e.ModbusDataType);
            };

            dataStore.DataStoreWrittenTo += (obj, e) => writtenToEventFired = true;

            dataStore.ReadData(dataStore.InputRegisters, 4, 0);
            Assert.True(readFromEventFired);
            Assert.False(writtenToEventFired);
        }

        [Fact]
        public void DataStoreReadFromEvent_ReadInputs()
        {
            DataStore dataStore = DataStoreFactory.CreateTestDataStore();

            bool readFromEventFired = false;
            bool writtenToEventFired = false;

            dataStore.DataStoreReadFrom += (obj, e) =>
            {
                readFromEventFired = true;
                Assert.Equal(4, e.StartAddress);
                Assert.Equal(new bool[] { false }, e.Data.A.ToArray());
                Assert.Equal(ModbusDataType.Input, e.ModbusDataType);
            };

            dataStore.DataStoreWrittenTo += (obj, e) => writtenToEventFired = true;

            dataStore.ReadData(dataStore.InputDiscretes, 4, 1);
            Assert.True(readFromEventFired);
            Assert.False(writtenToEventFired);
        }

        [Fact]
        public void DataStoreWrittenToEvent_WriteCoils()
        {
            DataStore dataStore = DataStoreFactory.CreateTestDataStore();

            bool readFromEventFired = false;
            bool writtenToEventFired = false;

            dataStore.DataStoreWrittenTo += (obj, e) =>
            {
                writtenToEventFired = true;
                Assert.Equal(3, e.Data.A.Count);
                Assert.Equal(4, e.StartAddress);
                Assert.Equal(new[] { true, false, true }, e.Data.A.ToArray());
                Assert.Equal(ModbusDataType.Coil, e.ModbusDataType);
            };

            dataStore.DataStoreReadFrom += (obj, e) => readFromEventFired = true;

            dataStore.WriteData(new DiscreteCollection(true, false, true), dataStore.CoilDiscretes, 4);
            Assert.False(readFromEventFired);
            Assert.True(writtenToEventFired);
        }

        [Fact]
        public void DataStoreWrittenToEvent_WriteHoldingRegisters()
        {
            DataStore dataStore = DataStoreFactory.CreateTestDataStore();

            bool readFromEventFired = false;
            bool writtenToEventFired = false;

            dataStore.DataStoreWrittenTo += (obj, e) =>
            {
                writtenToEventFired = true;
                Assert.Equal(3, e.Data.B.Count);
                Assert.Equal(0, e.StartAddress);
                Assert.Equal(new ushort[] { 5, 6, 7 }, e.Data.B.ToArray());
                Assert.Equal(ModbusDataType.HoldingRegister, e.ModbusDataType);
            };

            dataStore.DataStoreReadFrom += (obj, e) => readFromEventFired = true;

            dataStore.WriteData(new RegisterCollection(5, 6, 7), dataStore.HoldingRegisters, 0);
            Assert.False(readFromEventFired);
            Assert.True(writtenToEventFired);
        }

        [Fact]
        public void Update()
        {
            List<int> newItems = new List<int>(new int[] { 4, 5, 6 });
            List<int> destination = new List<int>(new int[] { 1, 2, 3, 7, 8, 9 });
            DataStore.Update<int>(newItems, destination, 3);
            Assert.Equal(new int[] { 1, 2, 3, 4, 5, 6 }, destination.ToArray());
        }

        [Fact]
        public void UpdateItemsTooLarge()
        {
            List<int> newItems = new List<int>(new int[] { 1, 2, 3, 7, 8, 9 });
            List<int> destination = new List<int>(new int[] { 4, 5, 6 });
            Assert.Throws<InvalidModbusRequestException>(() => DataStore.Update<int>(newItems, destination, 3));
        }

        [Fact]
        public void UpdateNegativeIndex()
        {
            List<int> newItems = new List<int>(new int[] { 1, 2, 3, 7, 8, 9 });
            List<int> destination = new List<int>(new int[] { 4, 5, 6 });
            Assert.Throws<InvalidModbusRequestException>(() => DataStore.Update<int>(newItems, destination, -1));
        }
    }
}