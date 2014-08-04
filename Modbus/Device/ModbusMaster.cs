using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Modbus.Data;
using Modbus.IO;
using Modbus.Message;

namespace Modbus.Device
{
    using Unme.Common;

    /// <summary>
    ///     Modbus master device.
    /// </summary>
    public abstract class ModbusMaster : ModbusDevice, IModbusMaster
    {
        internal ModbusMaster(ModbusTransport transport)
            : base(transport)
        {
        }

        /// <summary>
        ///     Read from 1 to 2000 contiguous coils status.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of coils to read.</param>
        /// <returns>Coils status</returns>
        public bool[] ReadCoils(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 2000);

            return ReadDiscretes(Modbus.ReadCoils, slaveAddress, startAddress, numberOfPoints);
        }

        /// <summary>
        ///     Read from 1 to 2000 contiguous discrete input status.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of discrete inputs to read.</param>
        /// <returns>Discrete inputs status</returns>
        public bool[] ReadInputs(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 2000);

            return ReadDiscretes(Modbus.ReadInputs, slaveAddress, startAddress, numberOfPoints);
        }

        /// <summary>
        ///     Read contiguous block of 16 bit holding registers.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>Holding registers status</returns>
        public ushort[] ReadHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 125);

            return ReadRegisters(Modbus.ReadHoldingRegisters, slaveAddress, startAddress, numberOfPoints);
        }

        /// <summary>
        ///     Read contiguous block of 16 bit input registers.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>Input registers status</returns>
        public ushort[] ReadInputRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 125);

            return ReadRegisters(Modbus.ReadInputRegisters, slaveAddress, startAddress, numberOfPoints);
        }

        /// <summary>
        ///     Write a single coil value.
        /// </summary>
        /// <param name="slaveAddress">Address of the device to write to.</param>
        /// <param name="coilAddress">Address to write value to.</param>
        /// <param name="value">Value to write.</param>
        public void WriteSingleCoil(byte slaveAddress, ushort coilAddress, bool value)
        {
            WriteSingleCoilRequestResponse request = new WriteSingleCoilRequestResponse(slaveAddress, coilAddress, value);
            Transport.UnicastMessage<WriteSingleCoilRequestResponse>(request);
        }

        /// <summary>
        ///     Write a single holding register.
        /// </summary>
        /// <param name="slaveAddress">Address of the device to write to.</param>
        /// <param name="registerAddress">Address to write.</param>
        /// <param name="value">Value to write.</param>
        public void WriteSingleRegister(byte slaveAddress, ushort registerAddress, ushort value)
        {
            WriteSingleRegisterRequestResponse request = new WriteSingleRegisterRequestResponse(slaveAddress,
                registerAddress, value);
            Transport.UnicastMessage<WriteSingleRegisterRequestResponse>(request);
        }

        /// <summary>
        ///     Write a block of 1 to 123 contiguous 16 bit holding registers.
        /// </summary>
        /// <param name="slaveAddress">Address of the device to write to.</param>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        public void WriteMultipleRegisters(byte slaveAddress, ushort startAddress, ushort[] data)
        {
            ValidateData("data", data, 123);

            WriteMultipleRegistersRequest request = new WriteMultipleRegistersRequest(slaveAddress, startAddress,
                new RegisterCollection(data));
            Transport.UnicastMessage<WriteMultipleRegistersResponse>(request);
        }

        /// <summary>
        ///     Force each coil in a sequence of coils to a provided value.
        /// </summary>
        /// <param name="slaveAddress">Address of the device to write to.</param>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        public void WriteMultipleCoils(byte slaveAddress, ushort startAddress, bool[] data)
        {
            ValidateData("data", data, 1968);

            WriteMultipleCoilsRequest request = new WriteMultipleCoilsRequest(slaveAddress, startAddress,
                new DiscreteCollection(data));
            Transport.UnicastMessage<WriteMultipleCoilsResponse>(request);
        }

        /// <summary>
        ///     Performs a combination of one read operation and one write operation in a single Modbus transaction.
        ///     The write operation is performed before the read.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startReadAddress">Address to begin reading (Holding registers are addressed starting at 0).</param>
        /// <param name="numberOfPointsToRead">Number of registers to read.</param>
        /// <param name="startWriteAddress">Address to begin writing (Holding registers are addressed starting at 0).</param>
        /// <param name="writeData">Register values to write.</param>
        public ushort[] ReadWriteMultipleRegisters(byte slaveAddress, ushort startReadAddress,
            ushort numberOfPointsToRead, ushort startWriteAddress, ushort[] writeData)
        {
            ValidateNumberOfPoints("numberOfPointsToRead", numberOfPointsToRead, 125);
            ValidateData("writeData", writeData, 121);

            ReadWriteMultipleRegistersRequest request = new ReadWriteMultipleRegistersRequest(slaveAddress,
                startReadAddress, numberOfPointsToRead, startWriteAddress, new RegisterCollection(writeData));
            ReadHoldingInputRegistersResponse response =
                Transport.UnicastMessage<ReadHoldingInputRegistersResponse>(request);

            return response.Data.ToArray();
        }

        /// <summary>
        ///     Executes the custom message.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")]
        public TResponse ExecuteCustomMessage<TResponse>(IModbusMessage request) where TResponse : IModbusMessage, new()
        {
            return Transport.UnicastMessage<TResponse>(request);
        }

        internal static void ValidateData<T>(string argumentName, T[] data, int maxDataLength)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length == 0 || data.Length > maxDataLength)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture,
                    "The length of argument {0} must be between 1 and {1} inclusive.",
                    argumentName,
                    maxDataLength));
            }
        }

        internal static void ValidateNumberOfPoints(string argumentName, ushort numberOfPoints, ushort maxNumberOfPoints)
        {
            if (numberOfPoints < 1 || numberOfPoints > maxNumberOfPoints)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture,
                    "Argument {0} must be between 1 and {1} inclusive.",
                    argumentName,
                    maxNumberOfPoints));
            }
        }

        internal ushort[] ReadRegisters(byte functionCode, byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ReadHoldingInputRegistersRequest request = new ReadHoldingInputRegistersRequest(functionCode, slaveAddress,
                startAddress, numberOfPoints);
            ReadHoldingInputRegistersResponse response =
                Transport.UnicastMessage<ReadHoldingInputRegistersResponse>(request);

            return response.Data.ToArray();
        }

        internal bool[] ReadDiscretes(byte functionCode, byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(functionCode, slaveAddress, startAddress,
                numberOfPoints);
            ReadCoilsInputsResponse response = Transport.UnicastMessage<ReadCoilsInputsResponse>(request);

            return response.Data.Slice(0, request.NumberOfPoints).ToArray();
        }
    }
}