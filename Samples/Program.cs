using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Modbus.Data;
using Modbus.Device;
using Modbus.Utility;
using Modbus.Serial;
using Modbus.Message;
using Samples.ModbusCustomMessage;

namespace MySample
{
    /// <summary>
    ///     Demonstration of NModbus
    /// </summary>
    public class Driver
    {
        private static void Main(string[] args)
        {
            try
            {
                //ModbusTcpMasterReadInputs();
                //SimplePerfTest();
                //ModbusSerialRtuMasterWriteRegisters();
                //ModbusSerialAsciiMasterReadRegisters();
                //ModbusTcpMasterReadInputs();
                //StartModbusAsciiSlave();
                //ModbusTcpMasterReadInputsFromModbusSlave();
                //ModbusSerialAsciiMasterReadRegistersFromModbusSlave();
                //StartModbusTcpSlave();
                //StartModbusUdpSlave();
                //StartModbusAsciiSlave();

                //Custom Message Example - Start
                //StartModbusTcpSlaveCustomFunctionHandler();
                //StartModbusTcpMasterCustomFunction();
                //Custom Message Example - End
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Starts a Modbus Master to request a custom function over modbus and receive the response
        /// </summary>
        private static void StartModbusTcpMasterCustomFunction()
        {
            //TcpClient client = new TcpClient(AddressFamily.InterNetwork);
            //client.SendTimeout = 1000;
            //client.ReceiveTimeout = 1000;
            //await client.ConnectAsync(amsIpAddress, _modbusPort);
            //masterAMS = ModbusIpMaster.CreateIp(client);

            try
            {
                using (TcpClient client = new TcpClient("127.0.0.1", 502))
                {
                    ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
                    //Generate a custom function 105 request
                    var request = new CustomFunctionModbusMessage();
                    request.SlaveAddress = 1;
                    request.FunctionCode = 105;
                    request.PayloadLength = 0;

                    var response = master.ExecuteCustomMessage<CustomFunctionModbusMessage>(request);
                    
                    Console.WriteLine($"Response function code received {response.FunctionCode}");
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine($"Exception occured while executing custom function, {exp.Message}");
            }
            
        }

        /// <summary>
        /// Starts a Modbus Slave with custom function handler
        /// It will use custom function 105 just as an example
        /// </summary>
        private static void StartModbusTcpSlaveCustomFunctionHandler()
        {
            byte slaveId = 1;
            int port = 502;
            IPAddress address = new IPAddress(new byte[] { 127, 0, 0, 1 });

            // create and start the TCP slave
            TcpListener slaveTcpListener = new TcpListener(address, port);
            slaveTcpListener.Start();

            ModbusSlave slave = ModbusTcpSlave.CreateTcp(slaveId, slaveTcpListener, onCreateCustomRequest, onCreateCustomResponse);
            slave.DataStore = DataStoreFactory.CreateDefaultDataStore();

            slave.ListenAsync().GetAwaiter().GetResult();

            // prevent the main thread from exiting
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Call back to create custom Modbus response object based to specific request
        /// </summary>
        /// <param name="request">Custom ModbusRequest object created in <see cref="onCreateCustomRequest(byte[])"/></param>
        /// <returns></returns>
        private static IModbusMessage onCreateCustomResponse(IModbusMessage request)
        {
            //create custom response, It will not do any thing special, will only check function code, if it is 105
            //then send response just with function code 105
            if(request.FunctionCode == 105)
            {
                //Custome function request received.
                var response = new CustomFunctionModbusMessage();
                response.Initialize(request.MessageFrame);
                //update and send back the custom response
                return response;
            }
            else
            {
                //Not implemented function
                throw new NotImplementedException($"The function code {request.FunctionCode} is not supported");
            }
        }

        /// <summary>
        /// Call back to create Modbus custom request object from raw message frame
        /// </summary>
        /// <param name="rawMessageFrame"></param>
        /// <returns></returns>
        private static IModbusMessage onCreateCustomRequest(byte[] rawMessageFrame)
        {
            var request = new CustomFunctionModbusMessage();
            request.Initialize(rawMessageFrame);
            return request;
        }

        /// <summary>
        ///     Simple Modbus serial RTU master write holding registers example.
        /// </summary>
        public static void ModbusSerialRtuMasterWriteRegisters()
        {
            using (SerialPort port = new SerialPort("COM1"))
            {
                // configure serial port
                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.Open();

                var adapter = new SerialPortAdapter(port);
                // create modbus master
                IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(adapter);

                byte slaveId = 1;
                ushort startAddress = 100;
                ushort[] registers = new ushort[] { 1, 2, 3 };

                // write three registers
                master.WriteMultipleRegisters(slaveId, startAddress, registers);
            }
        }

        /// <summary>
        ///     Simple Modbus serial ASCII master read holding registers example.
        /// </summary>
        public static void ModbusSerialAsciiMasterReadRegisters()
        {
            using (SerialPort port = new SerialPort("COM1"))
            {
                // configure serial port
                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.Open();

                var adapter = new SerialPortAdapter(port);
                // create modbus master
                IModbusSerialMaster master = ModbusSerialMaster.CreateAscii(adapter);

                byte slaveId = 1;
                ushort startAddress = 1;
                ushort numRegisters = 5;

                // read five registers		
                ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, numRegisters);

                for (int i = 0; i < numRegisters; i++)
                {
                    Console.WriteLine($"Register {startAddress + i}={registers[i]}");
                }
            }

            // output: 
            // Register 1=0
            // Register 2=0
            // Register 3=0
            // Register 4=0
            // Register 5=0
        }

        /// <summary>
        ///     Simple Modbus TCP master read inputs example.
        /// </summary>
        public static void ModbusTcpMasterReadInputs()
        {
            using (TcpClient client = new TcpClient("127.0.0.1", 502))
            {
                ModbusIpMaster master = ModbusIpMaster.CreateIp(client);

                // read five input values
                ushort startAddress = 100;
                ushort numInputs = 5;
                bool[] inputs = master.ReadInputs(startAddress, numInputs);

                for (int i = 0; i < numInputs; i++)
                {
                    Console.WriteLine($"Input {(startAddress + i)}={(inputs[i] ? 1 : 0)}");
                }
            }

            // output: 
            // Input 100=0
            // Input 101=0
            // Input 102=0
            // Input 103=0
            // Input 104=0
        }

        /// <summary>
        ///     Simple Modbus UDP master write coils example.
        /// </summary>
        public static void ModbusUdpMasterWriteCoils()
        {
            using (UdpClient client = new UdpClient())
            {
                IPEndPoint endPoint = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 502);
                client.Connect(endPoint);

                ModbusIpMaster master = ModbusIpMaster.CreateIp(client);

                ushort startAddress = 1;

                // write three coils
                master.WriteMultipleCoils(startAddress, new bool[] { true, false, true });
            }
        }

        /// <summary>
        ///     Simple Modbus serial ASCII slave example.
        /// </summary>
        public static void StartModbusSerialAsciiSlave()
        {
            using (SerialPort slavePort = new SerialPort("COM2"))
            {
                // configure serial port
                slavePort.BaudRate = 9600;
                slavePort.DataBits = 8;
                slavePort.Parity = Parity.None;
                slavePort.StopBits = StopBits.One;
                slavePort.Open();

                byte unitId = 1;

                var adapter = new SerialPortAdapter(slavePort);
                // create modbus slave
                ModbusSlave slave = ModbusSerialSlave.CreateAscii(unitId, adapter);
                slave.DataStore = DataStoreFactory.CreateDefaultDataStore();

                slave.ListenAsync().GetAwaiter().GetResult();
            }
        }

        /// <summary>
        ///     Simple Modbus serial RTU slave example.
        /// </summary>
        public static void StartModbusSerialRtuSlave()
        {
            using (SerialPort slavePort = new SerialPort("COM2"))
            {
                // configure serial port
                slavePort.BaudRate = 9600;
                slavePort.DataBits = 8;
                slavePort.Parity = Parity.None;
                slavePort.StopBits = StopBits.One;
                slavePort.Open();

                byte unitId = 1;

                var adapter = new SerialPortAdapter(slavePort);
                // create modbus slave
                ModbusSlave slave = ModbusSerialSlave.CreateRtu(unitId, adapter);
                slave.DataStore = DataStoreFactory.CreateDefaultDataStore();

                slave.ListenAsync().GetAwaiter().GetResult();
            }
        }

        /// <summary>
        ///     Simple Modbus serial USB ASCII slave example.
        /// </summary>
        public static void StartModbusSerialUsbAsciiSlave()
        {
            // TODO
        }

        /// <summary>
        ///     Simple Modbus serial USB RTU slave example.
        /// </summary>
        public static void StartModbusSerialUsbRtuSlave()
        {
            // TODO
        }

        /// <summary>
        ///     Simple Modbus TCP slave example.
        /// </summary>
        public static void StartModbusTcpSlave()
        {
            byte slaveId = 1;
            int port = 502;
            IPAddress address = new IPAddress(new byte[] { 127, 0, 0, 1 });

            // create and start the TCP slave
            TcpListener slaveTcpListener = new TcpListener(address, port);
            slaveTcpListener.Start();

            ModbusSlave slave = ModbusTcpSlave.CreateTcp(slaveId, slaveTcpListener);
            slave.DataStore = DataStoreFactory.CreateDefaultDataStore();

            slave.ListenAsync().GetAwaiter().GetResult();

            // prevent the main thread from exiting
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        ///     Simple Modbus UDP slave example.
        /// </summary>
        public static void StartModbusUdpSlave()
        {
            using (UdpClient client = new UdpClient(502))
            {
                ModbusUdpSlave slave = ModbusUdpSlave.CreateUdp(client);
                slave.DataStore = DataStoreFactory.CreateDefaultDataStore();

                slave.ListenAsync().GetAwaiter().GetResult();

                // prevent the main thread from exiting
                Thread.Sleep(Timeout.Infinite);
            }
        }

        /// <summary>
        ///     Modbus TCP master and slave example.
        /// </summary>
        public static void ModbusTcpMasterReadInputsFromModbusSlave()
        {
            byte slaveId = 1;
            int port = 502;
            IPAddress address = new IPAddress(new byte[] { 127, 0, 0, 1 });

            // create and start the TCP slave
            TcpListener slaveTcpListener = new TcpListener(address, port);
            slaveTcpListener.Start();
            ModbusSlave slave = ModbusTcpSlave.CreateTcp(slaveId, slaveTcpListener);
            var listenTask = slave.ListenAsync();

            // create the master
            TcpClient masterTcpClient = new TcpClient(address.ToString(), port);
            ModbusIpMaster master = ModbusIpMaster.CreateIp(masterTcpClient);

            ushort numInputs = 5;
            ushort startAddress = 100;

            // read five register values
            ushort[] inputs = master.ReadInputRegisters(startAddress, numInputs);

            for (int i = 0; i < numInputs; i++)
            {
                Console.WriteLine($"Register {(startAddress + i)}={(inputs[i])}");
            }

            // clean up
            masterTcpClient.Close();
            slaveTcpListener.Stop();

            // output
            // Register 100=0
            // Register 101=0
            // Register 102=0
            // Register 103=0
            // Register 104=0
        }

        /// <summary>
        ///     Modbus serial ASCII master and slave example.
        /// </summary>
        public static void ModbusSerialAsciiMasterReadRegistersFromModbusSlave()
        {
            using (SerialPort masterPort = new SerialPort("COM1"))
            using (SerialPort slavePort = new SerialPort("COM2"))
            {
                // configure serial ports
                masterPort.BaudRate = slavePort.BaudRate = 9600;
                masterPort.DataBits = slavePort.DataBits = 8;
                masterPort.Parity = slavePort.Parity = Parity.None;
                masterPort.StopBits = slavePort.StopBits = StopBits.One;
                masterPort.Open();
                slavePort.Open();

                var slaveAdapter = new SerialPortAdapter(slavePort);
                // create modbus slave on seperate thread
                byte slaveId = 1;
                ModbusSlave slave = ModbusSerialSlave.CreateAscii(slaveId, slaveAdapter);
                var listenTask = slave.ListenAsync();

                var masterAdapter = new SerialPortAdapter(masterPort);
                // create modbus master
                ModbusSerialMaster master = ModbusSerialMaster.CreateAscii(masterAdapter);

                master.Transport.Retries = 5;
                ushort startAddress = 100;
                ushort numRegisters = 5;

                // read five register values
                ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, numRegisters);

                for (int i = 0; i < numRegisters; i++)
                {
                    Console.WriteLine($"Register {(startAddress + i)}={registers[i]}");
                }
            }

            // output
            // Register 100=0
            // Register 101=0
            // Register 102=0
            // Register 103=0
            // Register 104=0
        }

        /// <summary>
        ///     Write a 32 bit value.
        /// </summary>
        public static void ReadWrite32BitValue()
        {
            using (SerialPort port = new SerialPort("COM1"))
            {
                // configure serial port
                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.Open();

                var adapter = new SerialPortAdapter(port);
                // create modbus master
                ModbusSerialMaster master = ModbusSerialMaster.CreateRtu(adapter);

                byte slaveId = 1;
                ushort startAddress = 1008;
                uint largeValue = UInt16.MaxValue + 5;

                ushort lowOrderValue = BitConverter.ToUInt16(BitConverter.GetBytes(largeValue), 0);
                ushort highOrderValue = BitConverter.ToUInt16(BitConverter.GetBytes(largeValue), 2);

                // write large value in two 16 bit chunks
                master.WriteMultipleRegisters(slaveId, startAddress, new ushort[] { lowOrderValue, highOrderValue });

                // read large value in two 16 bit chunks and perform conversion
                ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, 2);
                uint value = ModbusUtility.GetUInt32(registers[1], registers[0]);
            }
        }
    }
}
