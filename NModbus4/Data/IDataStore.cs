namespace Modbus.Data
{
    using System.Collections.Generic;

    public interface IDataStore
    {
        /// <summary>
        ///     Gets the discrete coils.
        /// </summary>
        ModbusDataCollection<bool> CoilDiscretes { get; }

        /// <summary>
        ///     Gets the discrete inputs.
        /// </summary>
        ModbusDataCollection<bool> InputDiscretes { get; }

        /// <summary>
        ///     Gets the holding registers.
        /// </summary>
        ModbusDataCollection<ushort> HoldingRegisters { get; }

        /// <summary>
        ///     Gets the input registers.
        /// </summary>
        ModbusDataCollection<ushort> InputRegisters { get; }

        /// <summary>
        ///     Retrieves subset of discrete data from collection
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="startAddress"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        DiscreteCollection ReadData(
            ModbusDataCollection<bool> dataSource,
            ushort startAddress,
            ushort count);

        /// <summary>
        ///     Retrieves subset of register data from collection
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="startAddress"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        RegisterCollection ReadData(
            ModbusDataCollection<ushort> dataSource,
            ushort startAddress,
            ushort count);

        /// <summary>
        ///     Write discrete data to data store
        /// </summary>
        /// <param name="items"></param>
        /// <param name="destination"></param>
        /// <param name="startAddress"></param>
        void WriteData(
            IEnumerable<bool> items,
            ModbusDataCollection<bool> destination,
            ushort startAddress);

        /// <summary>
        ///     Write register data to data store
        /// </summary>
        /// <param name="items"></param>
        /// <param name="destination"></param>
        /// <param name="startAddress"></param>
        void WriteData(
            IEnumerable<ushort> items,
            ModbusDataCollection<ushort> destination,
            ushort startAddress);
    }
}
