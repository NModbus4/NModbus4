namespace Modbus.Message
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;

    public class ReadFileRecordRequest : AbstractModbusMessage, IModbusRequest
    {
        public ReadFileRecordRequest()
        {
        }

        // numberOfPoints is the number of Records within the file in this context
        public ReadFileRecordRequest(byte slaveAddress, ushort fileNumber, ushort startRecord, ushort numberOfPoints)
            : base(slaveAddress, Modbus.ReadFileRecords)
        {
            FileNumber = fileNumber;
            StartRecord = startRecord;
            NumberOfPoints = numberOfPoints;
            ByteCount = 8; // only supporting one "sub-response", requires 7 bytes + 1 for the length byte
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

                return pdu.ToArray();
            }
        }

        public byte ByteCount
        {
            get { return MessageImpl.ByteCount.Value; }
            set { MessageImpl.ByteCount = value; }
        }

        public ushort FileNumber { get; set; }

        public ushort StartRecord 
        {
            get { return MessageImpl.StartAddress.Value; }
            set { MessageImpl.StartAddress = value; }
        }

        public override int MinimumFrameSize
        {
            get { return 6; }
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

        public override string ToString()
        {
            string msg = $"Read {NumberOfPoints} registers of file record {StartRecord} in file {FileNumber}.";
            return msg;
        }

        public void ValidateResponse(IModbusMessage response)
        {
            var typedResponse = response as ReadFileRecordResponse;
            Debug.Assert(typedResponse != null, "Argument response should be of type ReadFileRecordRequest.");
            var expectedByteCount = NumberOfPoints * 2;

            if (expectedByteCount != typedResponse.ByteCount)
            {
                string msg = $"Unexpected byte count. Expected {expectedByteCount}, received {typedResponse.ByteCount}.";
                throw new IOException(msg);
            }
        }

        protected override void InitializeUnique(byte[] frame)
        {
            FileNumber = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
            NumberOfPoints = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4));
        }
    }
}
