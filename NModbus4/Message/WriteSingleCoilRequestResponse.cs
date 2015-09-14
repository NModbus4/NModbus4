namespace Modbus.Message
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;

    using Data;

    using Unme.Common;

    /// <summary>
    ///
    /// </summary>
    public class WriteSingleCoilRequestResponse : AbstractModbusMessageWithData<RegisterCollection>, IModbusRequest
    {
        /// <summary>
        ///
        /// </summary>
        public WriteSingleCoilRequestResponse()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="coilState"></param>
        public WriteSingleCoilRequestResponse(byte slaveAddress, ushort startAddress, bool coilState)
            : base(slaveAddress, Modbus.WriteSingleCoil)
        {
            StartAddress = startAddress;
            Data = new RegisterCollection(coilState ? Modbus.CoilOn : Modbus.CoilOff);
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

            string msg = $"Write single coil {(Data.First() == Modbus.CoilOn ? 1 : 0)} at address {StartAddress}.";
            return msg;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="response"></param>
        public void ValidateResponse(IModbusMessage response)
        {
            var typedResponse = (WriteSingleCoilRequestResponse)response;

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
            Data = new RegisterCollection(frame.Slice(4, 2).ToArray());
        }
    }
}
