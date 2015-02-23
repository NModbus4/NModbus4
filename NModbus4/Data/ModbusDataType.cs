namespace Modbus.Data
{
    /// <summary>
    ///     Types of data supported by the Modbus protocol.
    /// </summary>
    public enum ModbusDataType
    {
        /// <summary>
        ///     read/write register
        /// </summary>
        HoldingRegister,

        /// <summary>
        ///     readonly register
        /// </summary>
        InputRegister,

        /// <summary>
        ///     read/write discrete
        /// </summary>
        Coil,

        /// <summary>
        ///     readonly discrete
        /// </summary>
        Input
    }
}
