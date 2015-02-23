namespace Modbus.Message
{
    using System;
    using System.Globalization;
    using System.IO;

    using Data;

    /// <summary>
    /// 
    /// </summary>
    public class ReadWriteMultipleRegistersRequest : AbstractModbusMessage, IModbusRequest
    {
        private ReadHoldingInputRegistersRequest _readRequest;
        private WriteMultipleRegistersRequest _writeRequest;

        /// <summary>
        /// 
        /// </summary>
        public ReadWriteMultipleRegistersRequest()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startReadAddress"></param>
        /// <param name="numberOfPointsToRead"></param>
        /// <param name="startWriteAddress"></param>
        /// <param name="writeData"></param>
        public ReadWriteMultipleRegistersRequest(byte slaveAddress, ushort startReadAddress, ushort numberOfPointsToRead,
            ushort startWriteAddress, RegisterCollection writeData)
            : base(slaveAddress, Modbus.ReadWriteMultipleRegisters)
        {
            _readRequest = new ReadHoldingInputRegistersRequest(Modbus.ReadHoldingRegisters, slaveAddress,
                startReadAddress, numberOfPointsToRead);
            _writeRequest = new WriteMultipleRegistersRequest(slaveAddress, startWriteAddress, writeData);
        }

        /// <summary>
        /// 
        /// </summary>
        public override byte[] ProtocolDataUnit
        {
            get
            {
                byte[] readPdu = _readRequest.ProtocolDataUnit;
                byte[] writePdu = _writeRequest.ProtocolDataUnit;
                var stream = new MemoryStream(readPdu.Length + writePdu.Length);

                stream.WriteByte(FunctionCode);
                // read and write PDUs without function codes
                stream.Write(readPdu, 1, readPdu.Length - 1);
                stream.Write(writePdu, 1, writePdu.Length - 1);

                return stream.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ReadHoldingInputRegistersRequest ReadRequest
        {
            get { return _readRequest; }
        }

        /// <summary>
        /// 
        /// </summary>
        public WriteMultipleRegistersRequest WriteRequest
        {
            get { return _writeRequest; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int MinimumFrameSize
        {
            get { return 11; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture,
                "Write {0} holding registers starting at address {1}, and read {2} registers starting at address {3}.",
                _writeRequest.NumberOfPoints,
                _writeRequest.StartAddress,
                _readRequest.NumberOfPoints,
                _readRequest.StartAddress);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        public void ValidateResponse(IModbusMessage response)
        {
            var typedResponse = (ReadHoldingInputRegistersResponse) response;

            var expectedByteCount = ReadRequest.NumberOfPoints*2;
            if (expectedByteCount != typedResponse.ByteCount)
            {
                throw new IOException(String.Format(CultureInfo.InvariantCulture,
                    "Unexpected byte count in response. Expected {0}, received {1}.",
                    expectedByteCount,
                    typedResponse.ByteCount));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        protected override void InitializeUnique(byte[] frame)
        {
            if (frame.Length < MinimumFrameSize + frame[10])
                throw new FormatException("Message frame does not contain enough bytes.");

            byte[] readFrame = new byte[2 + 4];
            byte[] writeFrame = new byte[frame.Length - 6 + 2];

            readFrame[0] = writeFrame[0] = SlaveAddress;
            readFrame[1] = writeFrame[1] = FunctionCode;

            Buffer.BlockCopy(frame, 2, readFrame, 2, 4);
            Buffer.BlockCopy(frame, 6, writeFrame, 2, frame.Length - 6);

            _readRequest = ModbusMessageFactory.CreateModbusMessage<ReadHoldingInputRegistersRequest>(readFrame);
            _writeRequest = ModbusMessageFactory.CreateModbusMessage<WriteMultipleRegistersRequest>(writeFrame);
        }
    }
}
