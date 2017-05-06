namespace Modbus.Device
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using Data;
    using IO;
    using Message;

    /// <summary>
    ///     Modbus slave device.
    /// </summary>
    public abstract class ModbusSlave : ModbusDevice
    {
        internal ModbusSlave(byte unitId, ModbusTransport transport)
            : base(transport)
        {
            DataStore = DataStoreFactory.CreateDefaultDataStore();
            UnitId = unitId;
        }

        /// <summary>
        ///     Raised when a Modbus slave receives a request, before processing request function.
        /// </summary>
        /// <exception cref="InvalidModbusRequestException">The Modbus request was invalid, and an error response the specified exception should be sent.</exception>
        public event EventHandler<ModbusSlaveRequestEventArgs> ModbusSlaveRequestReceived;

        /// <summary>
        ///     Raised when a Modbus slave receives a write request, after processing the write portion of the function.
        /// </summary>
        /// <remarks>For Read/Write Multiple registers (function code 23), this method is raised after writing and before reading.</remarks>
        public event EventHandler<ModbusSlaveRequestEventArgs> WriteComplete;

        /// <summary>
        /// Custom Request Creator, will be called when custom request is received. 
        /// </summary>
        protected Func<byte[], IModbusMessage> _customRequestCreator;

        /// <summary>
        /// Custom Response Creator, will be called when custom response needs to be generated
        /// </summary>
        protected Func<IModbusMessage, IModbusMessage> _customResponseCreator;

        /// <summary>
        ///     Gets or sets the data store.
        /// </summary>
        public DataStore DataStore { get; set; }

        /// <summary>
        ///     Gets or sets the unit ID.
        /// </summary>
        public byte UnitId { get; set; }

        /// <summary>
        ///     Start slave listening for requests.
        /// </summary>
        public abstract Task ListenAsync();

        internal static ReadCoilsInputsResponse ReadDiscretes(
            ReadCoilsInputsRequest request,
            DataStore dataStore,
            ModbusDataCollection<bool> dataSource)
        {
            DiscreteCollection data;
            ReadCoilsInputsResponse response;

            data = DataStore.ReadData<DiscreteCollection, bool>(
                dataStore,
                dataSource,
                request.StartAddress,
                request.NumberOfPoints,
                dataStore.SyncRoot);

            response = new ReadCoilsInputsResponse(
                request.FunctionCode,
                request.SlaveAddress,
                data.ByteCount,
                data);

            return response;
        }

        internal static ReadHoldingInputRegistersResponse ReadRegisters(
            ReadHoldingInputRegistersRequest request,
            DataStore dataStore,
            ModbusDataCollection<ushort> dataSource)
        {
            RegisterCollection data;
            ReadHoldingInputRegistersResponse response;

            data = DataStore.ReadData<RegisterCollection, ushort>(
                dataStore,
                dataSource,
                request.StartAddress,
                request.NumberOfPoints,
                dataStore.SyncRoot);

            response = new ReadHoldingInputRegistersResponse(
                request.FunctionCode,
                request.SlaveAddress,
                data);

            return response;
        }

        internal static WriteSingleCoilRequestResponse WriteSingleCoil(
            WriteSingleCoilRequestResponse request,
            DataStore dataStore,
            ModbusDataCollection<bool> dataSource)
        {
            DataStore.WriteData(
                dataStore,
                new DiscreteCollection(request.Data[0] == Modbus.CoilOn),
                dataSource,
                request.StartAddress,
                dataStore.SyncRoot);

            return request;
        }

        internal static WriteMultipleCoilsResponse WriteMultipleCoils(
            WriteMultipleCoilsRequest request,
            DataStore dataStore,
            ModbusDataCollection<bool> dataSource)
        {
            WriteMultipleCoilsResponse response;

            DataStore.WriteData(
                dataStore,
                request.Data.Take(request.NumberOfPoints),
                dataSource,
                request.StartAddress,
                dataStore.SyncRoot);

            response = new WriteMultipleCoilsResponse(
                request.SlaveAddress,
                request.StartAddress,
                request.NumberOfPoints);

            return response;
        }

        internal static WriteSingleRegisterRequestResponse WriteSingleRegister(
            WriteSingleRegisterRequestResponse request,
            DataStore dataStore,
            ModbusDataCollection<ushort> dataSource)
        {
            DataStore.WriteData(
                dataStore,
                request.Data,
                dataSource,
                request.StartAddress,
                dataStore.SyncRoot);

            return request;
        }

        internal static WriteMultipleRegistersResponse WriteMultipleRegisters(
            WriteMultipleRegistersRequest request,
            DataStore dataStore,
            ModbusDataCollection<ushort> dataSource)
        {
            WriteMultipleRegistersResponse response;

            DataStore.WriteData(
                dataStore,
                request.Data,
                dataSource,
                request.StartAddress,
                dataStore.SyncRoot);

            response = new WriteMultipleRegistersResponse(
                request.SlaveAddress,
                request.StartAddress,
                request.NumberOfPoints);

            return response;
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Cast is not unneccessary.")]
        internal IModbusMessage ApplyRequest(IModbusMessage request)
        {
            IModbusMessage response;

            try
            {
                Debug.WriteLine(request.ToString());
                var eventArgs = new ModbusSlaveRequestEventArgs(request);
                ModbusSlaveRequestReceived?.Invoke(this, eventArgs);

                switch (request.FunctionCode)
                {
                    case Modbus.ReadCoils:
                        response = ReadDiscretes(
                            (ReadCoilsInputsRequest)request,
                            DataStore,
                            DataStore.CoilDiscretes);
                        break;
                    case Modbus.ReadInputs:
                        response = ReadDiscretes(
                            (ReadCoilsInputsRequest)request,
                            DataStore,
                            DataStore.InputDiscretes);
                        break;
                    case Modbus.ReadHoldingRegisters:
                        response = ReadRegisters(
                            (ReadHoldingInputRegistersRequest)request,
                            DataStore,
                            DataStore.HoldingRegisters);
                        break;
                    case Modbus.ReadInputRegisters:
                        response = ReadRegisters(
                            (ReadHoldingInputRegistersRequest)request,
                            DataStore,
                            DataStore.InputRegisters);
                        break;
                    case Modbus.Diagnostics:
                        response = request;
                        break;
                    case Modbus.WriteSingleCoil:
                        response = WriteSingleCoil(
                            (WriteSingleCoilRequestResponse)request,
                            DataStore,
                            DataStore.CoilDiscretes);
                        WriteComplete?.Invoke(this, eventArgs);
                        break;
                    case Modbus.WriteSingleRegister:
                        response = WriteSingleRegister(
                            (WriteSingleRegisterRequestResponse)request,
                            DataStore,
                            DataStore.HoldingRegisters);
                        WriteComplete?.Invoke(this, eventArgs);
                        break;
                    case Modbus.WriteMultipleCoils:
                        response = WriteMultipleCoils(
                            (WriteMultipleCoilsRequest)request,
                            DataStore,
                            DataStore.CoilDiscretes);
                        WriteComplete?.Invoke(this, eventArgs);
                        break;
                    case Modbus.WriteMultipleRegisters:
                        response = WriteMultipleRegisters(
                            (WriteMultipleRegistersRequest)request,
                            DataStore,
                            DataStore.HoldingRegisters);
                        WriteComplete?.Invoke(this, eventArgs);
                        break;
                    case Modbus.ReadWriteMultipleRegisters:
                        ReadWriteMultipleRegistersRequest readWriteRequest = (ReadWriteMultipleRegistersRequest)request;
                        WriteMultipleRegisters(
                            readWriteRequest.WriteRequest,
                            DataStore,
                            DataStore.HoldingRegisters);
                        WriteComplete?.Invoke(this, eventArgs);
                        response = ReadRegisters(
                            readWriteRequest.ReadRequest,
                            DataStore,
                            DataStore.HoldingRegisters);
                        break;
                    default:
                        //Seems to be a custom request, create custom response
                        response = ApplyCustomRequest(request);
                        break;
                        //string msg = $"Unsupported function code {request.FunctionCode}.";
                        //Debug.WriteLine(msg);
                        //throw new InvalidModbusRequestException(Modbus.IllegalFunction);
                }
            }
            catch (InvalidModbusRequestException ex)
            {
                // Catches the exception for an illegal function or a custom exception from the ModbusSlaveRequestReceived event.
                response = new SlaveExceptionResponse(
                    request.SlaveAddress,
                    (byte)(Modbus.ExceptionOffset + request.FunctionCode),
                    ex.ExceptionCode);
            }

            return response;
        }

        /// <summary>
        /// Calls a handler for custom messages
        /// </summary>
        /// <param name="messageFrame"></param>
        /// <returns></returns>
        internal Message.IModbusMessage OnCustomRequestReceived(byte[] messageFrame)
        {
            if (_customRequestCreator == null)
            {
                string msg = $"Unsupported function code {messageFrame[1]}, Consider using proper overload to provide handler for custom request creator";
                Debug.WriteLine(msg);
                throw new ArgumentException(msg, nameof(messageFrame));
            }
            else
            {
                return _customRequestCreator.Invoke(messageFrame);
            }
        }

        /// <summary>
        /// Calls a Handler for a custom request and generate 
        /// valid Modbus custom response (If needs to be handled in different way)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response of provided Modbus request</returns>
        internal IModbusMessage ApplyCustomRequest(IModbusMessage request)
        {
            if (_customResponseCreator == null)
            {
                string msg = $"Unsupported function code {request.FunctionCode}, Consider using proper overload to provide handler for custom response creator";
                Debug.WriteLine(msg);
                throw new InvalidModbusRequestException(Modbus.IllegalFunction);
            }
            else
            {
                return _customResponseCreator.Invoke(request);
            }
        }
    }
}
