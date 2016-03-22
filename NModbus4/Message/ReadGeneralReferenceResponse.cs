using System;
using System.Linq;
using Modbus.Data;
using Modbus.Unme.Common;

namespace Modbus.Message
{
    public class ReadGeneralReferenceResponse : AbstractModbusMessageWithData<RegisterCollection>, IModbusMessage
    {
        public override int MinimumFrameSize => 3;

        protected override void InitializeUnique(byte[] frame)
        {

            var byteCount = frame[2];
            var byteCountFirstGroup = frame[3];
            var referenceTypeFirstGroup = frame[4];

            this.Data = new RegisterCollection(frame.Skip(5).ToArray());

        }
    }
}