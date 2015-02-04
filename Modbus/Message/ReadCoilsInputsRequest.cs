using System;
using System.Globalization;
using System.IO;
using System.Net;

namespace Modbus.Message
{
    internal class ReadCoilsInputsRequest : AbstractModbusMessage, IModbusRequest
    {
        public ReadCoilsInputsRequest()
        {
        }

        public ReadCoilsInputsRequest(byte functionCode, byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            : base(slaveAddress, functionCode)
        {
            StartAddress = startAddress;
            NumberOfPoints = numberOfPoints;
        }

        public ushort StartAddress
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
            get { return MessageImpl.NumberOfPoints.Value; }
            set
            {
                if (value > Modbus.MaximumDiscreteRequestResponseSize)
                    throw new ArgumentOutOfRangeException("NumberOfPoints",
                        String.Format(CultureInfo.InvariantCulture, "Maximum amount of data {0} coils.",
                            Modbus.MaximumDiscreteRequestResponseSize));

                MessageImpl.NumberOfPoints = value;
            }
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "Read {0} {1} starting at address {2}.", NumberOfPoints,
                FunctionCode == Modbus.ReadCoils ? "coils" : "inputs", StartAddress);
        }

        public void ValidateResponse(IModbusMessage response)
        {
            var typedResponse = (ReadCoilsInputsResponse) response;

            // best effort validation - the same response for a request for 1 vs 6 coils (same byte count) will pass validation.
            var expectedByteCount = (NumberOfPoints + 7)/8;
            if (expectedByteCount != typedResponse.ByteCount)
            {
                throw new IOException(String.Format(CultureInfo.InvariantCulture,
                    "Unexpected byte count. Expected {0}, received {1}.",
                    expectedByteCount,
                    typedResponse.ByteCount));
            }
        }

        protected override void InitializeUnique(byte[] frame)
        {
            StartAddress = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
            NumberOfPoints = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4));
        }
    }
}