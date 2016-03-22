using System;
using System.Collections.Generic;
using System.Net;

namespace Modbus.Message
{
    public class ReadGeneralReferenceRequest : IModbusMessage
    {
        public ushort NumberOfPoints { get; }
        public byte FunctionCode { get; set; }
        private byte[] referenceNumber;
        private byte referenceType;

        public ReadGeneralReferenceRequest(byte functionCode, byte slaveAddress, byte referenceType, byte[] referenceNumber, ushort numberOfPoints)
        {
            this.FunctionCode = functionCode;
            this.SlaveAddress = slaveAddress;
            this.referenceType = referenceType;
            this.referenceNumber = referenceNumber;
            this.NumberOfPoints = numberOfPoints;
        }

        public ReadGeneralReferenceRequest()
        {
        }

        public byte SlaveAddress { get; set; }
        public byte[] MessageFrame { get; }

        public byte[] ProtocolDataUnit
        {
            get
            {
                var result = new List<byte>();

                result.Add(FunctionCode);
                result.Add(7);
                result.Add(referenceType);
                result.AddRange(referenceNumber);
                result.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)NumberOfPoints)));
                return result.ToArray();
            }
        }

        public ushort TransactionId { get; set; }
        public void Initialize(byte[] frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame), "Argument frame cannot be null.");
            }

            if (frame.Length < Modbus.MinimumFrameSize)
            {
                string msg = $"Message frame must contain at least {Modbus.MinimumFrameSize} bytes of data.";
                throw new FormatException(msg);
            }

            SlaveAddress = frame[0];
            FunctionCode = frame[1];
        }
    }
}