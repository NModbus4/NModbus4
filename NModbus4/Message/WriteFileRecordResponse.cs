namespace Modbus.Message
{
    using System;
    using System.Net;
    using System.Linq;
    using Unme.Common;

    public class WriteFileRecordResponse : AbstractModbusMessage, IModbusMessage
    {
        public WriteFileRecordResponse()
        {
        }

        public WriteFileRecordResponse(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            : base(slaveAddress, Modbus.WriteFileRecords)
        {
            StartAddress = startAddress;
            NumberOfPoints = numberOfPoints;
        }

        // I'm overridding MessageFrame, because it seems like we ought to be returning the actual
        // message we got instead of recreating it when checking the crc
        private byte[] _messageFrame;
        public override byte[] MessageFrame
        {
            get
            {
                return _messageFrame;
            }
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

        public ushort StartAddress
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
            string msg = $"Wrote {NumberOfPoints} registers in file record.";
            return msg;
        }

        protected override void InitializeUnique(byte[] frame)
        {
            StartAddress = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 6));
            NumberOfPoints = frame[2];
            _messageFrame = frame.Slice(0, frame.Length - 2).ToArray();
        }
    }
}
