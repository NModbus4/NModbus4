namespace Modbus.Message
{
    using System;
    using System.IO;
    using System.Net;

    /// <summary>
    ///
    /// </summary>
    public class ReadCoilsInputsRequest : AbstractModbusMessage, IModbusRequest
    {
        /// <summary>
        ///
        /// </summary>
        public ReadCoilsInputsRequest()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="functionCode"></param>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        public ReadCoilsInputsRequest(byte functionCode, byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            : base(slaveAddress, functionCode)
        {
            StartAddress = startAddress;
            NumberOfPoints = numberOfPoints;
        }

        /// <summary>
        ///
        /// </summary>
        public ushort StartAddress
        {
            get { return MessageImpl.StartAddress.Value; }
            set { MessageImpl.StartAddress = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public override int MinimumFrameSize
        {
            get { return 6; }
        }

        /// <summary>
        ///
        /// </summary>
        public ushort NumberOfPoints
        {
            get
            {
                return MessageImpl.NumberOfPoints.Value;
            }
            set
            {
                if (value > Modbus.MaximumDiscreteRequestResponseSize)
                {
                    string msg = $"Maximum amount of data {Modbus.MaximumDiscreteRequestResponseSize} coils.";
                    throw new ArgumentOutOfRangeException("NumberOfPoints", msg);
                }

                MessageImpl.NumberOfPoints = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string msg = $"Read {NumberOfPoints} {(FunctionCode == Modbus.ReadCoils ? "coils" : "inputs")} starting at address {StartAddress}.";
            return msg;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="response"></param>
        public void ValidateResponse(IModbusMessage response)
        {
            var typedResponse = (ReadCoilsInputsResponse)response;

            // best effort validation - the same response for a request for 1 vs 6 coils (same byte count) will pass validation.
            var expectedByteCount = (NumberOfPoints + 7) / 8;

            if (expectedByteCount != typedResponse.ByteCount)
            {
                string msg = $"Unexpected byte count. Expected {expectedByteCount}, received {typedResponse.ByteCount}.";
                throw new IOException(msg);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="frame"></param>
        protected override void InitializeUnique(byte[] frame)
        {
            StartAddress = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
            NumberOfPoints = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4));
        }
    }
}
