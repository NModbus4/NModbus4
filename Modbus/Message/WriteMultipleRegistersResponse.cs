using System;
using System.Globalization;
using System.Net;

namespace Modbus.Message
{
    internal class WriteMultipleRegistersResponse : AbstractModbusMessage, IModbusMessage
    {
        public WriteMultipleRegistersResponse()
        {
        }

        public WriteMultipleRegistersResponse(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            : base(slaveAddress, Modbus.WriteMultipleRegisters)
        {
            StartAddress = startAddress;
            NumberOfPoints = numberOfPoints;
        }

        public ushort NumberOfPoints
        {
            get { return MessageImpl.NumberOfPoints.Value; }
            set
            {
                if (value > Modbus.MaximumRegisterRequestResponseSize)
                {
                    throw new ArgumentOutOfRangeException("NumberOfPoints",
                        String.Format(CultureInfo.InvariantCulture, "Maximum amount of data {0} registers.",
                            Modbus.MaximumRegisterRequestResponseSize));
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
            get { return 6; }
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "Wrote {0} holding registers starting at address {1}.",
                NumberOfPoints, StartAddress);
        }

        protected override void InitializeUnique(byte[] frame)
        {
            StartAddress = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
            NumberOfPoints = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4));
        }
    }
}