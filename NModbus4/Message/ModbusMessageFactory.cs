namespace Modbus.Message
{
    using System;
    using System.Globalization;

    /// <summary>
    /// 
    /// </summary>
    public static class ModbusMessageFactory
    {
        private const int MinRequestFrameLength = 3;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static T CreateModbusMessage<T>(byte[] frame) where T : IModbusMessage, new()
        {
            IModbusMessage message = new T();
            message.Initialize(frame);

            return (T) message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static IModbusMessage CreateModbusRequest(byte[] frame)
        {
            if (frame.Length < MinRequestFrameLength)
                throw new FormatException(String.Format(CultureInfo.InvariantCulture,
                    "Argument 'frame' must have a length of at least {0} bytes.", MinRequestFrameLength));

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
                    throw new ArgumentException(
                        String.Format(CultureInfo.InvariantCulture, "Unsupported function code {0}", functionCode),
                        "frame");
            }

            return request;
        }
    }
}
