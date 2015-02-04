using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Modbus.Data;

namespace Modbus.Message
{
    using Unme.Common;

    internal class ReadWriteMultipleRegistersRequest : AbstractModbusMessage, IModbusRequest
    {
        private ReadHoldingInputRegistersRequest _readRequest;
        private WriteMultipleRegistersRequest _writeRequest;

        public ReadWriteMultipleRegistersRequest()
        {
        }

        public ReadWriteMultipleRegistersRequest(byte slaveAddress, ushort startReadAddress, ushort numberOfPointsToRead,
            ushort startWriteAddress, RegisterCollection writeData)
            : base(slaveAddress, Modbus.ReadWriteMultipleRegisters)
        {
            _readRequest = new ReadHoldingInputRegistersRequest(Modbus.ReadHoldingRegisters, slaveAddress,
                startReadAddress, numberOfPointsToRead);
            _writeRequest = new WriteMultipleRegistersRequest(slaveAddress, startWriteAddress, writeData);
        }

        public override byte[] ProtocolDataUnit
        {
            get
            {
                // read and write PDUs without function codes
                byte[] read = _readRequest.ProtocolDataUnit.Slice(1, _readRequest.ProtocolDataUnit.Length - 1).ToArray();
                byte[] write =
                    _writeRequest.ProtocolDataUnit.Slice(1, _writeRequest.ProtocolDataUnit.Length - 1).ToArray();

                return FunctionCode.ToSequence().Concat(read, write).ToArray();
            }
        }

        public ReadHoldingInputRegistersRequest ReadRequest
        {
            get { return _readRequest; }
        }

        public WriteMultipleRegistersRequest WriteRequest
        {
            get { return _writeRequest; }
        }

        public override int MinimumFrameSize
        {
            get { return 11; }
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture,
                "Write {0} holding registers starting at address {1}, and read {2} registers starting at address {3}.",
                _writeRequest.NumberOfPoints,
                _writeRequest.StartAddress,
                _readRequest.NumberOfPoints,
                _readRequest.StartAddress);
        }

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

        protected override void InitializeUnique(byte[] frame)
        {
            if (frame.Length < MinimumFrameSize + frame[10])
                throw new FormatException("Message frame does not contain enough bytes.");

            byte[] readFrame = frame.Slice(2, 4).ToArray();
            byte[] writeFrame = frame.Slice(6, frame.Length - 6).ToArray();
            byte[] header = {SlaveAddress, FunctionCode};

            _readRequest =
                ModbusMessageFactory.CreateModbusMessage<ReadHoldingInputRegistersRequest>(
                    header.Concat(readFrame).ToArray());
            _writeRequest =
                ModbusMessageFactory.CreateModbusMessage<WriteMultipleRegistersRequest>(
                    header.Concat(writeFrame).ToArray());
        }
    }
}