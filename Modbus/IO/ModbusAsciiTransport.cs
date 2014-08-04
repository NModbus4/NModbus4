using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Modbus.Message;
using Modbus.Utility;

namespace Modbus.IO
{
    using Unme.Common;

    /// <summary>
    ///     Refined Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    internal class ModbusAsciiTransport : ModbusSerialTransport
    {
        internal ModbusAsciiTransport(IStreamResource streamResource)
            : base(streamResource)
        {
            Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
        }

        internal override byte[] BuildMessageFrame(IModbusMessage message)
        {
            var frame = new List<byte>();
            frame.Add((byte) ':');
            frame.AddRange(ModbusUtility.GetAsciiBytes(message.SlaveAddress));
            frame.AddRange(ModbusUtility.GetAsciiBytes(message.ProtocolDataUnit));
            frame.AddRange(ModbusUtility.GetAsciiBytes(ModbusUtility.CalculateLrc(message.MessageFrame)));
            frame.AddRange(Encoding.ASCII.GetBytes(Modbus.NewLine.ToCharArray()));

            return frame.ToArray();
        }

        internal override bool ChecksumsMatch(IModbusMessage message, byte[] messageFrame)
        {
            return ModbusUtility.CalculateLrc(message.MessageFrame) == messageFrame[messageFrame.Length - 1];
        }

        internal override byte[] ReadRequest()
        {
            return ReadRequestResponse();
        }

        internal override IModbusMessage ReadResponse<T>()
        {
            return CreateResponse<T>(ReadRequestResponse());
        }

        internal byte[] ReadRequestResponse()
        {
            // read message frame, removing frame start ':'
            string frameHex = StreamResourceUtility.ReadLine(StreamResource).Substring(1);

            // convert hex to bytes
            byte[] frame = ModbusUtility.HexToBytes(frameHex);
            Debug.WriteLine("RX: {0}", frame.Join(", "));

            if (frame.Length < 3)
                throw new IOException("Premature end of stream, message truncated.");

            return frame;
        }
    }
}