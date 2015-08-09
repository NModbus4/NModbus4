namespace Modbus.IO
{
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    using Message;
    using Utility;

    /// <summary>
    ///     Refined Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    internal class ModbusRtuTransport : ModbusSerialTransport
    {
        public const int RequestFrameStartLength = 7;
        public const int ResponseFrameStartLength = 4;

        internal ModbusRtuTransport(IStreamResource streamResource)
            : base(streamResource)
        {
            Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
        }

        public static int RequestBytesToRead(byte[] frameStart)
        {
            byte functionCode = frameStart[1];
            int numBytes;

            switch (functionCode)
            {
                case ModbusConstants.ReadCoils:
                case ModbusConstants.ReadInputs:
                case ModbusConstants.ReadHoldingRegisters:
                case ModbusConstants.ReadInputRegisters:
                case ModbusConstants.WriteSingleCoil:
                case ModbusConstants.WriteSingleRegister:
                case ModbusConstants.Diagnostics:
                    numBytes = 1;
                    break;
                case ModbusConstants.WriteMultipleCoils:
                case ModbusConstants.WriteMultipleRegisters:
                    byte byteCount = frameStart[6];
                    numBytes = byteCount + 2;
                    break;
                default:
                    string errorMessage = String.Format(CultureInfo.InvariantCulture, "Function code {0} not supported.",
                        functionCode);
                    Debug.WriteLine(errorMessage);
                    throw new NotImplementedException(errorMessage);
            }

            return numBytes;
        }

        public static int ResponseBytesToRead(byte[] frameStart)
        {
            byte functionCode = frameStart[1];

            // exception response
            if (functionCode > ModbusConstants.ExceptionOffset)
                return 1;

            int numBytes;
            switch (functionCode)
            {
                case ModbusConstants.ReadCoils:
                case ModbusConstants.ReadInputs:
                case ModbusConstants.ReadHoldingRegisters:
                case ModbusConstants.ReadInputRegisters:
                    numBytes = frameStart[2] + 1;
                    break;
                case ModbusConstants.WriteSingleCoil:
                case ModbusConstants.WriteSingleRegister:
                case ModbusConstants.WriteMultipleCoils:
                case ModbusConstants.WriteMultipleRegisters:
                case ModbusConstants.Diagnostics:
                    numBytes = 4;
                    break;
                default:
                    string errorMessage = String.Format(CultureInfo.InvariantCulture, "Function code {0} not supported.",
                        functionCode);
                    Debug.WriteLine(errorMessage);
                    throw new NotImplementedException(errorMessage);
            }

            return numBytes;
        }

        public virtual byte[] Read(int count)
        {
            byte[] frameBytes = new byte[count];
            int numBytesRead = 0;

            while (numBytesRead != count)
                numBytesRead += StreamResource.Read(frameBytes, numBytesRead, count - numBytesRead);

            return frameBytes;
        }

        internal override byte[] BuildMessageFrame(IModbusMessage message)
        {
            var messageFrame = message.MessageFrame;
            var crc = ModbusUtility.CalculateCrc(messageFrame);
            var messageBody = new MemoryStream(messageFrame.Length + crc.Length);

            messageBody.Write(messageFrame, 0, messageFrame.Length);
            messageBody.Write(crc, 0, crc.Length);

            return messageBody.ToArray();
        }

        internal override bool ChecksumsMatch(IModbusMessage message, byte[] messageFrame)
        {
            return BitConverter.ToUInt16(messageFrame, messageFrame.Length - 2) ==
                   BitConverter.ToUInt16(ModbusUtility.CalculateCrc(message.MessageFrame), 0);
        }

        internal override IModbusMessage ReadResponse<T>()
        {
            byte[] frameStart = Read(ResponseFrameStartLength);
            byte[] frameEnd = Read(ResponseBytesToRead(frameStart));
            byte[] frame = Enumerable.Concat(frameStart, frameEnd).ToArray();
            Debug.WriteLine("RX: {0}", string.Join(", ", frame));

            return CreateResponse<T>(frame);
        }

        internal override byte[] ReadRequest()
        {
            byte[] frameStart = Read(RequestFrameStartLength);
            byte[] frameEnd = Read(RequestBytesToRead(frameStart));
            byte[] frame = Enumerable.Concat(frameStart, frameEnd).ToArray();
            Debug.WriteLine("RX: {0}", string.Join(", ", frame));

            return frame;
        }
    }
}
