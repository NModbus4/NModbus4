namespace Modbus.Message
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    using Data;

    using Unme.Common;

    public class WriteFileRecordRequest : AbstractModbusMessageWithData<RegisterCollection>, IModbusRequest
    {
        public WriteFileRecordRequest()
        {
        }

        public WriteFileRecordRequest(byte slaveAddress, ushort fileNumber, ushort startRecord, RegisterCollection data)
            : base(slaveAddress, Modbus.WriteFileRecords)
        {
            FileNumber = fileNumber;
            StartRecord = startRecord;
            NumberOfPoints = (ushort)data.Count;
            ByteCount = (byte)(8 + data.Count * 2); // only supporting one "sub-response", requires 7 bytes + 1 for the length byte
            Data = data;
        }

        // Even though this is the same as the function it overrides I think it still needs to be here so that it calls
        // the correct version of ProtocolDataUnit.  Worth testing.
        public override byte[] MessageFrame
        {
            get
            {
                var pdu = ProtocolDataUnit;
                var frame = new MemoryStream(1 + pdu.Length);
                frame.WriteByte(SlaveAddress);
                frame.Write(pdu, 0, pdu.Length);
                return frame.ToArray();
            }
        }

        public override byte[] ProtocolDataUnit
        {
            get
            {
                List<byte> pdu = new List<byte>();

                pdu.Add(FunctionCode);
                pdu.Add(ByteCount);
                pdu.Add(0x06); // sub-function code
                pdu.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)FileNumber)));
                pdu.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)StartRecord)));
                pdu.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)NumberOfPoints)));
                pdu.AddRange(Data.NetworkBytes);

                return pdu.ToArray();
            }
        }

        public byte ByteCount
        {
            get { return MessageImpl.ByteCount.Value; }
            set { MessageImpl.ByteCount = value; }
        }

        public ushort NumberOfPoints
        {
            get
            {
                return MessageImpl.NumberOfPoints.Value;
            }

            set
            {
                if (value > Modbus.MaximumRegisterRequestResponseSize)
                {
                    string msg = $"Maximum amount of data {Modbus.MaximumRegisterRequestResponseSize} registers.";
                    throw new ArgumentOutOfRangeException(nameof(NumberOfPoints), msg);
                }

                MessageImpl.NumberOfPoints = value;
            }
        }

        public ushort FileNumber { get; set; }

        public ushort StartRecord
        {
            get { return MessageImpl.StartAddress.Value; }
            set { MessageImpl.StartAddress = value; }
        }

        public override int MinimumFrameSize
        {
            get { return 8; }
        }

        public override string ToString()
        {
            string msg = $"Write {NumberOfPoints} registers of file record {StartRecord} in file {FileNumber}.";
            return msg;
        }

        public void ValidateResponse(IModbusMessage response)
        {
            var typedResponse = (WriteFileRecordResponse)response;

            // Write file record message just returns the exact same message back
            if (!this.MessageFrame.SequenceEqual(typedResponse.MessageFrame))
            {
                string msg = $"Unexpected response. Expected \n{string.Join(",", this.MessageFrame)}\n received \n{string.Join(",", typedResponse.MessageFrame)}";
                throw new IOException(msg);
            }
        }

        protected override void InitializeUnique(byte[] frame)
        {
            if (frame.Length < MinimumFrameSize + frame[6])
            {
                throw new FormatException("Message frame does not contain enough bytes.");
            }

            StartRecord = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
            NumberOfPoints = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4));
            ByteCount = frame[6];
            Data = new RegisterCollection(frame.Slice(7, ByteCount).ToArray());
        }
    }
}
