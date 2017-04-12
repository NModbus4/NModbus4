namespace Modbus.Device
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
#if SERIAL
    using System.IO.Ports;
#endif
    using IO;
    using Message;

    /// <summary>
    ///     Modbus serial slave device.
    /// </summary>
    public class ModbusSerialSlave : ModbusSlave
    {
        private ModbusSerialSlave(byte unitId, ModbusTransport transport)
            : base(unitId, transport)
        {
        }

        private ModbusSerialTransport SerialTransport
        {
            get
            {
                var transport = Transport as ModbusSerialTransport;

                if (transport == null)
                {
                    throw new ObjectDisposedException("SerialTransport");
                }

                return transport;
            }
        }

#if SERIAL
        /// <summary>
        ///     Modbus ASCII slave factory method.
        /// </summary>
        public static ModbusSerialSlave CreateAscii(byte unitId, SerialPort serialPort)
        {
            if (serialPort == null)
            {
                throw new ArgumentNullException(nameof(serialPort));
            }

            return CreateAscii(unitId, new SerialPortAdapter(serialPort));
        }

        /// <summary>
        ///     Modbus ASCII slave factory method.
        /// </summary>
        public static ModbusSerialSlave CreateAscii(byte unitId, SerialPort serialPort,
        Func<byte[], IModbusMessage> customRequestCreator, Func<IModbusMessage, IModbusMessage> customResponseCreator)
        {
            if (serialPort == null)
            {
                throw new ArgumentNullException(nameof(serialPort));
            }

            var slave = new CreateAscii(unitId, new SerialPortAdapter(serialPort));
            slave._customRequestCreator = customRequestCreator;
            slave._customResponseCreator = customResponseCreator;
            return slave;
        }
#endif

        /// <summary>
        ///     Modbus ASCII slave factory method.
        /// </summary>
        public static ModbusSerialSlave CreateAscii(byte unitId, IStreamResource streamResource)
        {
            if (streamResource == null)
            {
                throw new ArgumentNullException(nameof(streamResource));
            }

            return new ModbusSerialSlave(unitId, new ModbusAsciiTransport(streamResource));
        }

        /// <summary>
        ///     Modbus ASCII slave factory method.
        /// <param name="unitId"></param>
        /// <param name="streamResource"></param>
        /// <param name="customRequestCreator">CustomMessageHandler which will be called when a custom request is received</param>
        /// <param name="customResponseCreator">Custom Response generator, will be called when custom response is required against a custom request</param>
        /// </summary>
        public static ModbusSerialSlave CreateAscii(byte unitId, IStreamResource streamResource,
            Func<byte[], IModbusMessage> customRequestCreator, Func<IModbusMessage, IModbusMessage> customResponseCreator)
        {
            if (streamResource == null)
            {
                throw new ArgumentNullException(nameof(streamResource));
            }

            var slave = new ModbusSerialSlave(unitId, new ModbusAsciiTransport(streamResource));
            slave._customRequestCreator = customRequestCreator;
            slave._customResponseCreator = customResponseCreator;
            return slave;
        }

#if SERIAL
        /// <summary>
        ///     Modbus RTU slave factory method.
        /// </summary>
        public static ModbusSerialSlave CreateRtu(byte unitId, SerialPort serialPort)
        {
            if (serialPort == null)
            {
                throw new ArgumentNullException(nameof(serialPort));
            }

            return CreateRtu(unitId, new SerialPortAdapter(serialPort));
        }

        /// <summary>
        ///     Modbus RTU slave factory method.
        /// </summary>
        public static ModbusSerialSlave CreateRtu(byte unitId, SerialPort serialPort,
        Func<byte[], IModbusMessage> customRequestCreator, Func<IModbusMessage, IModbusMessage> customResponseCreator)
        {
            if (serialPort == null)
            {
                throw new ArgumentNullException(nameof(serialPort));
            }

            var slave = new CreateRtu(unitId, new SerialPortAdapter(serialPort));
            slave._customRequestCreator = customRequestCreator;
            slave._customResponseCreator = customResponseCreator;
            return slave;
        }
#endif

        /// <summary>
        ///     Modbus RTU slave factory method.
        /// </summary>
        public static ModbusSerialSlave CreateRtu(byte unitId, IStreamResource streamResource)
        {
            if (streamResource == null)
            {
                throw new ArgumentNullException(nameof(streamResource));
            }

            return new ModbusSerialSlave(unitId, new ModbusRtuTransport(streamResource));
        }

        /// <summary>
        ///     Modbus RTU slave factory method.
        /// </summary>
        public static ModbusSerialSlave CreateRtu(byte unitId, IStreamResource streamResource,
            Func<byte[], IModbusMessage> customRequestCreator, Func<IModbusMessage, IModbusMessage> customResponseCreator)
        {
            if (streamResource == null)
            {
                throw new ArgumentNullException(nameof(streamResource));
            }

            var slave = new ModbusSerialSlave(unitId, new ModbusRtuTransport(streamResource));
            slave._customRequestCreator = customRequestCreator;
            slave._customResponseCreator = customResponseCreator;
            return slave;
        }

        /// <summary>
        ///     Start slave listening for requests.
        /// </summary>
        public override async Task ListenAsync()
        {
            while (true)
            {
                try
                {
                    try
                    {
                        //TODO: remove deleay once async will be implemented in transport level
                        await Task.Delay(20).ConfigureAwait(false);

                        // read request and build message
                        byte[] frame = SerialTransport.ReadRequest();
                        
                        IModbusMessage request = ModbusMessageFactory.CreateModbusRequest(frame, this);
                        
                        if (SerialTransport.CheckFrame && !SerialTransport.ChecksumsMatch(request, frame))
                        {
                            string msg = $"Checksums failed to match {string.Join(", ", request.MessageFrame)} != {string.Join(", ", frame)}.";
                            Debug.WriteLine(msg);
                            throw new IOException(msg);
                        }

                        // only service requests addressed to this particular slave
                        if (request.SlaveAddress != UnitId)
                        {
                            Debug.WriteLine($"NModbus Slave {UnitId} ignoring request intended for NModbus Slave {request.SlaveAddress}");
                            continue;
                        }

                        // perform action
                        IModbusMessage response = ApplyRequest(request);

                        // write response
                        SerialTransport.Write(response);
                    }
                    catch (IOException ioe)
                    {
                        Debug.WriteLine($"IO Exception encountered while listening for requests - {ioe.Message}");
                        SerialTransport.DiscardInBuffer();
                    }
                    catch (TimeoutException te)
                    {
                        Debug.WriteLine($"Timeout Exception encountered while listening for requests - {te.Message}");
                        SerialTransport.DiscardInBuffer();
                    }

                    // TODO better exception handling here, missing FormatException, NotImplemented...
                }
                catch (InvalidOperationException)
                {
                    // when the underlying transport is disposed
                    break;
                }
            }
        }
    }
}
