namespace Modbus.Message
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Data;

    using Unme.Common;
    using System.IO;
    using System.Net;

    public class ReadFileRecordResponse : AbstractModbusMessageWithData<StringData>, IModbusMessage
    {
        public ReadFileRecordResponse()
        {
        }

        public ReadFileRecordResponse(byte slaveAddress, StringData data)
            : base(slaveAddress, Modbus.ReadFileRecords)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            ByteCount = data.ByteCount;
            Data = data;
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

        public byte ByteCount
        {
            get { return MessageImpl.ByteCount.Value; }
            set { MessageImpl.ByteCount = value; }
        }

        public byte FileResponseLength
        {
            get { return (byte)(ByteCount - 3); }
        }

        public override int MinimumFrameSize
        {
            get { return 3; }
        }

        public override string ToString()
        {
            string msg = $"Read file record.";
            return msg;
        }

        protected override void InitializeUnique(byte[] frame)
        {
            if (frame.Length < MinimumFrameSize + frame[2])
            {
                throw new FormatException("Message frame does not contain enough bytes.");
            }

            ByteCount = (byte)(frame[3] - 1);
            Data = new StringData(frame.Slice(5, ByteCount).ToArray());
            _messageFrame = frame.Slice(0, frame.Length - 2).ToArray();
        }
    }
}
