namespace Modbus.Message
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;

    using Data;

    using Unme.Common;

    /// <summary>
    /// 
    /// </summary>
    public class WriteMultipleCoilsRequest : AbstractModbusMessageWithData<DiscreteCollection>, IModbusRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public WriteMultipleCoilsRequest()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="data"></param>
        public WriteMultipleCoilsRequest(byte slaveAddress, ushort startAddress, DiscreteCollection data)
            : base(slaveAddress, ModbusConstants.WriteMultipleCoils)
        {
            StartAddress = startAddress;
            NumberOfPoints = (ushort) data.Count;
            ByteCount = (byte) ((data.Count + 7)/8);
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        public byte ByteCount
        {
            get { return MessageImpl.ByteCount.Value; }
            set { MessageImpl.ByteCount = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ushort NumberOfPoints
        {
            get { return MessageImpl.NumberOfPoints.Value; }
            set
            {
                if (value > ModbusConstants.MaximumDiscreteRequestResponseSize)
                    throw new ArgumentOutOfRangeException("NumberOfPoints",
                        String.Format(CultureInfo.InvariantCulture, "Maximum amount of data {0} coils.",
                            ModbusConstants.MaximumDiscreteRequestResponseSize));

                MessageImpl.NumberOfPoints = value;
            }
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
            get { return 7; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "Write {0} coils starting at address {1}.",
                NumberOfPoints, StartAddress);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        public void ValidateResponse(IModbusMessage response)
        {
            var typedResponse = (WriteMultipleCoilsResponse) response;

            if (StartAddress != typedResponse.StartAddress)
            {
                throw new IOException(String.Format(CultureInfo.InvariantCulture,
                    "Unexpected start address in response. Expected {0}, received {1}.",
                    StartAddress,
                    typedResponse.StartAddress));
            }

            if (NumberOfPoints != typedResponse.NumberOfPoints)
            {
                throw new IOException(String.Format(CultureInfo.InvariantCulture,
                    "Unexpected number of points in response. Expected {0}, received {1}.",
                    NumberOfPoints,
                    typedResponse.NumberOfPoints));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        protected override void InitializeUnique(byte[] frame)
        {
            if (frame.Length < MinimumFrameSize + frame[6])
                throw new FormatException("Message frame does not contain enough bytes.");

            StartAddress = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
            NumberOfPoints = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4));
            ByteCount = frame[6];
            Data = new DiscreteCollection(frame.Slice(7, ByteCount).ToArray());
        }
    }
}
