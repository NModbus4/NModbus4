namespace Modbus.IO
{
    using System;

    using Logging;

    using Message;

    /// <summary>
    ///     Transport for Internet protocols with detailed logging
    ///     Refined Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    internal class ModbusIpTransportWithLogging : ModbusIpTransport
    {
        private readonly INmodbusLogSink _logger;

        internal ModbusIpTransportWithLogging(IStreamResource streamResource, INmodbusLogSink logger)
            : base(streamResource)
        {
            _logger = logger;
        }

        internal override void Write(IModbusMessage message)
        {
            message.TransactionId = GetNewTransactionId();
            byte[] frame = BuildMessageFrame(message);
            _logger.Log(Severity.Debug, string.Format("TX: {0}", BitConverter.ToString(frame)));
            StreamResource.Write(frame, 0, frame.Length);
        }

        internal override byte[] ReadRequest()
        {
            var response = ReadRequestResponse(StreamResource);
            _logger.Log(Severity.Debug, string.Format("RX: {0}", BitConverter.ToString(response)));
            return response;
        }

        internal override IModbusMessage ReadResponse<T>()
        {
            var response = ReadRequestResponse(StreamResource);
            _logger.Log(Severity.Debug, string.Format("RX: {0}", BitConverter.ToString(response)));
            return CreateMessageAndInitializeTransactionId<T>(response);
        }
    }
}