namespace Modbus.Device
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Ports;
    using System.Net.Sockets;

    using IO;

    using Logging;

    /// <summary>
    /// Modbus IP master device for detailed transport logging
    /// </summary>
    public class ModbusIpMasterWithLogging : ModbusIpMaster
    {
        internal ModbusIpMasterWithLogging(ModbusTransport transport)
            : base(transport)
        {
        }

        /// <summary>
        ///    Modbus IP master factory method.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification = "Breaking change.")]
        public static ModbusIpMaster CreateIp(TcpClient tcpClient, INmodbusLogSink logger)
        {
            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");

            if (logger == null)
                throw new ArgumentNullException("logger");

            return CreateIp(new TcpClientAdapter(tcpClient), logger);
        }

        /// <summary>
        ///    Modbus IP master factory method.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification = "Breaking change.")]
        public static ModbusIpMaster CreateIp(UdpClient udpClient, INmodbusLogSink logger)
        {
            if (udpClient == null)
                throw new ArgumentNullException("udpClient");

            if (!udpClient.Client.Connected)
                throw new InvalidOperationException(Resources.UdpClientNotConnected);

            if (logger == null)
                throw new ArgumentNullException("logger");

            return CreateIp(new UdpClientAdapter(udpClient), logger);
        }

        /// <summary>
        ///     Modbus IP master factory method.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification = "Breaking change.")]
        public static ModbusIpMaster CreateIp(SerialPort serialPort, INmodbusLogSink logger)
        {
            if (serialPort == null)
                throw new ArgumentNullException("serialPort");

            if (logger == null)
                throw new ArgumentNullException("logger");

            return CreateIp(new SerialPortAdapter(serialPort), logger);
        }

        /// <summary>
        ///     Modbus IP master factory method.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification = "Breaking change.")]
        public static ModbusIpMaster CreateIp(IStreamResource streamResource, INmodbusLogSink logger)
        {
            if (streamResource == null)
                throw new ArgumentNullException("streamResource");

            if (logger == null)
                throw new ArgumentNullException("logger");

            return new ModbusIpMasterWithLogging(new ModbusIpTransportWithLogging(streamResource, logger));
        }
    }
}