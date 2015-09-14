namespace Modbus.Message
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;

    using Data;

    /// <summary>
    ///
    /// </summary>
    public class WriteSingleRegisterRequestResponse : AbstractModbusMessageWithData<RegisterCollection>, IModbusRequest
    {
        /// <summary>
        ///
        /// </summary>
        public WriteSingleRegisterRequestResponse()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="registerValue"></param>
        public WriteSingleRegisterRequestResponse(byte slaveAddress, ushort startAddress, ushort registerValue)
            : base(slaveAddress, Modbus.WriteSingleRegister)
        {
            StartAddress = startAddress;
            Data = new RegisterCollection(registerValue);
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
        public ushort StartAddress
        {
            get { return MessageImpl.StartAddress.Value; }
            set { MessageImpl.StartAddress = value; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            Debug.Assert(Data != null, "Argument Data cannot be null.");
            Debug.Assert(Data.Count() == 1, "Data should have a count of 1.");

            string msg = $"Write single holding register {Data[0]} at address {StartAddress}.";
            return msg;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="response"></param>
        public void ValidateResponse(IModbusMessage response)
        {
            var typedResponse = (WriteSingleRegisterRequestResponse)response;

            if (StartAddress != typedResponse.StartAddress)
            {
                string msg = $"Unexpected start address in response. Expected {StartAddress}, received {typedResponse.StartAddress}.";
                throw new IOException(msg);
            }

            if (Data.First() != typedResponse.Data.First())
            {
                string msg = $"Unexpected data in response. Expected {Data.First()}, received {typedResponse.Data.First()}.";
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
            Data = new RegisterCollection((ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4)));
        }
    }
}
