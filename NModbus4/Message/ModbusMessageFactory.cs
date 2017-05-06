namespace Modbus.Message
{
    using global::Modbus.Device;
    using System;

    /// <summary>
    ///     Modbus message factory.
    /// </summary>
    public static class ModbusMessageFactory
    {
        /// <summary>
        ///     Minimum request frame length.
        /// </summary>
        private const int MinRequestFrameLength = 3;

        /// <summary>
        ///     Create a Modbus message.
        /// </summary>
        /// <typeparam name="T">Modbus message type.</typeparam>
        /// <param name="frame">Bytes of Modbus frame.</param>
        /// <returns>New Modbus message based on type and frame bytes.</returns>
        public static T CreateModbusMessage<T>(byte[] frame)
            where T : IModbusMessage, new()
        {
            IModbusMessage message = new T();
            message.Initialize(frame);

            return (T)message;
        }

        /// <summary>
        ///     Create a Modbus request.
        /// </summary>
        /// <param name="frame">Bytes of Modbus frame.</param>
        /// <param name="slave">In case custom request/response handler needs to be provided</param>
        /// <returns>Modbus request.</returns>
        public static IModbusMessage CreateModbusRequest(byte[] frame, ModbusSlave slave = null)
        {
            if (frame.Length < MinRequestFrameLength)
            {
                string msg = $"Argument 'frame' must have a length of at least {MinRequestFrameLength} bytes.";
                throw new FormatException(msg);
            }

            IModbusMessage request;
            byte functionCode = frame[1];

            switch (functionCode)
            {
                case Modbus.ReadCoils:
                case Modbus.ReadInputs:
                    request = CreateModbusMessage<ReadCoilsInputsRequest>(frame);
                    break;
                case Modbus.ReadHoldingRegisters:
                case Modbus.ReadInputRegisters:
                    request = CreateModbusMessage<ReadHoldingInputRegistersRequest>(frame);
                    break;
                case Modbus.WriteSingleCoil:
                    request = CreateModbusMessage<WriteSingleCoilRequestResponse>(frame);
                    break;
                case Modbus.WriteSingleRegister:
                    request = CreateModbusMessage<WriteSingleRegisterRequestResponse>(frame);
                    break;
                case Modbus.Diagnostics:
                    request = CreateModbusMessage<DiagnosticsRequestResponse>(frame);
                    break;
                case Modbus.WriteMultipleCoils:
                    request = CreateModbusMessage<WriteMultipleCoilsRequest>(frame);
                    break;
                case Modbus.WriteMultipleRegisters:
                    request = CreateModbusMessage<WriteMultipleRegistersRequest>(frame);
                    break;
                case Modbus.ReadWriteMultipleRegisters:
                    request = CreateModbusMessage<ReadWriteMultipleRegistersRequest>(frame);
                    break;
                default:
                    if(slave == null)
                    {
                        string msg = $"Unsupported function code {frame[1]}";
                        throw new ArgumentException(msg, nameof(frame));
                    }
                    //seems like a custom request, call custom creator
                    request = slave.OnCustomRequestReceived(frame);
                    break;
            }

            return request;
        }
    }
}
