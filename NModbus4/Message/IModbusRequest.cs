namespace Modbus.Message
{
    /// <summary>
    ///     Methods specific to a modbus request message.
    /// </summary>
    public interface IModbusRequest : IModbusMessage
    {
        /// <summary>
        ///     Validate the specified response against the current request.
        /// </summary>
        /// <param name="response"></param>
        void ValidateResponse(IModbusMessage response);
    }
}
