using Modbus.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.ModbusCustomMessage
{
    public class CustomFunctionModbusMessage : IModbusMessage
    {
        #region DataMembers
        private const int _minimumFrameSize = 3;
        #endregion

        private byte _functionCode;
        /// <summary>
        /// Gets or Sets function code
        /// </summary>
        public byte FunctionCode
        {
            get
            {
                return _functionCode;
            }

            set
            {
                _functionCode = value;
            }
        }

        private byte _slaveAddress;
        /// <summary>
        /// Gets or Sets Unit Id
        /// </summary>
        public byte SlaveAddress
        {
            get
            {
                return _slaveAddress;
            }

            set
            {
                _slaveAddress = value;
            }
        }

        public byte[] MessageFrame
        {
            get
            {
                var pdu = ProtocolDataUnit;
                var frame = new MemoryStream(1 + pdu.Length);

                frame.WriteByte(SlaveAddress);
                frame.Write(pdu, 0, pdu.Length);

                return frame.ToArray();
            }
        }

        private byte[] _protocolDataUnit;
        public byte[] ProtocolDataUnit
        {
            get
            {
                //prepare the PDU
                List<byte> _pdu = new List<byte>();
                _pdu.Add(FunctionCode);
                _pdu.Add(PayloadLength);
                //We can prepare PDU here any custom response
                _protocolDataUnit = _pdu.ToArray();
                return _protocolDataUnit;
            }
        }

        private ushort _transactionId;
        /// <summary>
        /// Gets or Sets Transaction Id
        /// </summary>
        public ushort TransactionId
        {
            get
            {
                return _transactionId;
            }

            set
            {
                _transactionId = value;
            }
        }

        private byte _payloadLength;

        public byte PayloadLength
        {
            get { return _payloadLength; }
            set
            {
                _payloadLength = value;
            }
        }

        public void Initialize(byte[] frame)
        {
            //frame contains all the response bytes from UnitId (Slave address) to end
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame), "Argument frame cannot be null.");
            }

            if (frame.Length < _minimumFrameSize)
            {
                string msg = $"Message frame must contain at least {_minimumFrameSize} bytes of data.";
                throw new FormatException(msg);
            }
            SlaveAddress = frame[0];
            FunctionCode = frame[1];
            PayloadLength = frame[2];
        }
    }
}
