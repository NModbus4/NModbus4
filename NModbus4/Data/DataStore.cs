namespace Modbus.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Unme.Common;

    /// <summary>
    ///     Object simulation of device memory map.
    ///     The underlying collections are thread safe when using the ModbusMaster API to read/write values.
    ///     You can use the SyncRoot property to synchronize direct access to the DataStore collections.
    /// </summary>
    public class DataStore : IDataStore
    {
        private readonly object _syncRoot = new object();

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataStore" /> class.
        /// </summary>
        public DataStore()
        {
            CoilDiscretes = new ModbusDataCollection<bool> { ModbusDataType = ModbusDataType.Coil };
            InputDiscretes = new ModbusDataCollection<bool> { ModbusDataType = ModbusDataType.Input };
            HoldingRegisters = new ModbusDataCollection<ushort> { ModbusDataType = ModbusDataType.HoldingRegister };
            InputRegisters = new ModbusDataCollection<ushort> { ModbusDataType = ModbusDataType.InputRegister };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataStore"/> class.
        /// </summary>
        /// <param name="coilDiscretes">List of discrete coil values.</param>
        /// <param name="inputDiscretes">List of discrete input values</param>
        /// <param name="holdingRegisters">List of holding register values.</param>
        /// <param name="inputRegisters">List of input register values.</param>
        internal DataStore(
            IList<bool> coilDiscretes,
            IList<bool> inputDiscretes,
            IList<ushort> holdingRegisters,
            IList<ushort> inputRegisters)
        {
            CoilDiscretes = new ModbusDataCollection<bool>(coilDiscretes) { ModbusDataType = ModbusDataType.Coil };
            InputDiscretes = new ModbusDataCollection<bool>(inputDiscretes) { ModbusDataType = ModbusDataType.Input };
            HoldingRegisters = new ModbusDataCollection<ushort>(holdingRegisters) { ModbusDataType = ModbusDataType.HoldingRegister };
            InputRegisters = new ModbusDataCollection<ushort>(inputRegisters) { ModbusDataType = ModbusDataType.InputRegister };
        }

        /// <summary>
        ///     Occurs when the DataStore is written to via a Modbus command.
        /// </summary>
        public event EventHandler<DataStoreEventArgs> DataStoreWrittenTo;

        /// <summary>
        ///     Occurs when the DataStore is read from via a Modbus command.
        /// </summary>
        public event EventHandler<DataStoreEventArgs> DataStoreReadFrom;

        /// <summary>
        ///     Gets the discrete coils.
        /// </summary>
        public ModbusDataCollection<bool> CoilDiscretes { get; }

        /// <summary>
        ///     Gets the discrete inputs.
        /// </summary>
        public ModbusDataCollection<bool> InputDiscretes { get; }

        /// <summary>
        ///     Gets the holding registers.
        /// </summary>
        public ModbusDataCollection<ushort> HoldingRegisters { get; }

        /// <summary>
        ///     Gets the input registers.
        /// </summary>
        public ModbusDataCollection<ushort> InputRegisters { get; }

        /// <summary>
        ///     An object that can be used to synchronize direct access to the DataStore collections.
        /// </summary>
        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        public virtual DiscreteCollection ReadData(
            ModbusDataCollection<bool> dataSource,
            ushort startAddress,
            ushort count)
        {
            DataStoreEventArgs dataStoreEventArgs;
            int startIndex = startAddress + 1;

            if (startIndex < 0 || dataSource.Count < startIndex + count)
            {
                throw new InvalidModbusRequestException(Modbus.IllegalDataAddress);
            }

            bool[] dataToRetrieve;
            lock(SyncRoot)
            {
                dataToRetrieve = dataSource.Slice(startIndex, count).ToArray();
            }

            DiscreteCollection result = new DiscreteCollection();
            for (int i = 0; i < count; i++)
            {
                result.Add(dataToRetrieve[i]);
            }

            dataStoreEventArgs = DataStoreEventArgs.CreateDataStoreEventArgs(startAddress, dataSource.ModbusDataType, result);
            DataStoreReadFrom?.Invoke(this, dataStoreEventArgs);
            return result;
        }

        public virtual RegisterCollection ReadData(
            ModbusDataCollection<ushort> dataSource,
            ushort startAddress,
            ushort count)
        {
            DataStoreEventArgs dataStoreEventArgs;
            int startIndex = startAddress + 1;

            if (startIndex < 0 || dataSource.Count < startIndex + count)
            {
                throw new InvalidModbusRequestException(Modbus.IllegalDataAddress);
            }

            ushort[] dataToRetrieve;
            lock (SyncRoot)
            {
                dataToRetrieve = dataSource.Slice(startIndex, count).ToArray();
            }

            RegisterCollection result = new RegisterCollection();
            for (int i = 0; i < count; i++)
            {
                result.Add(dataToRetrieve[i]);
            }

            dataStoreEventArgs = DataStoreEventArgs.CreateDataStoreEventArgs(startAddress, dataSource.ModbusDataType, result);
            DataStoreReadFrom?.Invoke(this, dataStoreEventArgs);
            return result;
        }

        public virtual void WriteData(
            IEnumerable<bool> items,
            ModbusDataCollection<bool> destination,
            ushort startAddress)
        {
            DataStoreEventArgs dataStoreEventArgs;
            int startIndex = startAddress + 1;

            if (startIndex < 0 || destination.Count < startIndex + items.Count())
            {
                throw new InvalidModbusRequestException(Modbus.IllegalDataAddress);
            }

            lock(SyncRoot)
            {
                Update(items, destination, startIndex);
            }

            dataStoreEventArgs = DataStoreEventArgs.CreateDataStoreEventArgs(
                startAddress,
                destination.ModbusDataType,
                items);

            DataStoreWrittenTo?.Invoke(this, dataStoreEventArgs);
        }

        public virtual void WriteData(
            IEnumerable<ushort> items,
            ModbusDataCollection<ushort> destination,
            ushort startAddress)
        {
            DataStoreEventArgs dataStoreEventArgs;
            int startIndex = startAddress + 1;

            if (startIndex < 0 || destination.Count < startIndex + items.Count())
            {
                throw new InvalidModbusRequestException(Modbus.IllegalDataAddress);
            }

            lock (SyncRoot)
            {
                Update(items, destination, startIndex);
            }

            dataStoreEventArgs = DataStoreEventArgs.CreateDataStoreEventArgs(
                startAddress,
                destination.ModbusDataType,
                items);

            DataStoreWrittenTo?.Invoke(this, dataStoreEventArgs);
        }

        internal static void Update<T>(IEnumerable<T> items, IList<T> destination, int startIndex)
        {
            if (startIndex < 0 || destination.Count < startIndex + items.Count())
            {
                throw new InvalidModbusRequestException(Modbus.IllegalDataAddress);
            }

            int index = startIndex;

            foreach (T item in items)
            {
                destination[index] = item;
                ++index;
            }
        }
    }
}
