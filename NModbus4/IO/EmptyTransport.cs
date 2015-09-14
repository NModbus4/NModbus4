namespace Modbus.IO
{
    using System;

    using Message;

    /// <summary>
    ///     Empty placeholder.
    /// </summary>
    public class EmptyTransport : ModbusTransport
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal override byte[] ReadRequest()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal override IModbusMessage ReadResponse<T>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        internal override byte[] BuildMessageFrame(Message.IModbusMessage message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        internal override void Write(IModbusMessage message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        internal override void OnValidateResponse(IModbusMessage request, IModbusMessage response)
        {
            throw new NotImplementedException();
        }
    }
}
