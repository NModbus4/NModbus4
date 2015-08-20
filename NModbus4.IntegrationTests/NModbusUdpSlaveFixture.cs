using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Modbus.Data;
using Modbus.Device;
using Xunit;

namespace Modbus.IntegrationTests
{
    public class NModbusUdpSlaveFixture
    {
        [Fact]
        public async Task ModbusUdpSlave_EnsureTheSlaveShutsDownCleanly()
        {
            const int Timeout = 100;
            Task slaveListenTask;
            UdpClient client = new UdpClient(ModbusMasterFixture.Port);
            using (var slave = ModbusUdpSlave.CreateUdp(1, client))
            {
                var handle = new AutoResetEvent(false);

                slaveListenTask = Task.Run(() =>
                {
                    handle.Set();
                    slave.Listen();
                });

                handle.WaitOne();
                await Task.Delay(Timeout).ConfigureAwait(false);
            }

            await slaveListenTask.ConfigureAwait(false);
        }

        [Fact]
        public void ModbusUdpSlave_NotBound()
        {
            UdpClient client = new UdpClient();
            ModbusSlave slave = ModbusUdpSlave.CreateUdp(1, client);
            Assert.Throws<InvalidOperationException>(() => slave.Listen());
        }

        [Fact]
        public async Task ModbusUdpSlave_MultipleMasters()
        {
            const int TriesCount = 5;
            Random randomNumberGenerator = new Random();
            var master1Delays = new List<int>(TriesCount);
            var master2Delays = new List<int>(TriesCount);
            for (int i = 0; i < TriesCount; ++i)
            {
                master1Delays.Add(randomNumberGenerator.Next(1000));
                master2Delays.Add(randomNumberGenerator.Next(1000));
            }

            UdpClient masterClient1 = new UdpClient();
            masterClient1.Connect(ModbusMasterFixture.DefaultModbusIPEndPoint);
            ModbusIpMaster master1 = ModbusIpMaster.CreateIp(masterClient1);

            UdpClient masterClient2 = new UdpClient();
            masterClient2.Connect(ModbusMasterFixture.DefaultModbusIPEndPoint);
            ModbusIpMaster master2 = ModbusIpMaster.CreateIp(masterClient2);

            Task slaveTask = CreateAndStartUdpSlave(ModbusMasterFixture.Port, DataStoreFactory.CreateTestDataStore());
            using (ModbusSlave slaveClient = (ModbusSlave)slaveTask.AsyncState)
            {
                Task master1Thread = Task.Run(async () =>
                {
                    for (int i = 0; i < 5; i++)
                    {
                        await Task.Delay(master1Delays[i]).ConfigureAwait(false);
                        Debug.WriteLine("Read from master 1");
                        Assert.Equal(new ushort[] { 2, 3, 4, 5, 6 }, master1.ReadHoldingRegisters(1, 5));
                    }
                });

                Task master2Thread = Task.Run(async () =>
                {
                    for (int i = 0; i < 5; i++)
                    {
                        await Task.Delay(master2Delays[i]).ConfigureAwait(false);
                        Debug.WriteLine("Read from master 2");
                        Assert.Equal(new ushort[] { 3, 4, 5, 6, 7 }, master2.ReadHoldingRegisters(2, 5));
                    }
                });

                await master1Thread.ConfigureAwait(false);
                await master2Thread.ConfigureAwait(false);

                masterClient1.Close();
                masterClient2.Close();
            }

            await slaveTask.ConfigureAwait(false);
        }

        [Fact]
        public async Task ModbusUdpSlave_MultiThreaded()
        {
            var dataStore = DataStoreFactory.CreateDefaultDataStore();
            dataStore.CoilDiscretes.Add(false);

            Task slaveTask = CreateAndStartUdpSlave(502, dataStore);
            using (ModbusSlave slave = (ModbusSlave)slaveTask.AsyncState)
            {
                var workerThread1 = Task.Run((Func<Task>)ReadThread);
                var workerThread2 = Task.Run((Func<Task>)ReadThread);

                await workerThread1.ConfigureAwait(false);
                await workerThread2.ConfigureAwait(false);
            }

            await slaveTask.ConfigureAwait(false);
        }

        private static async Task ReadThread()
        {
            var masterClient = new UdpClient();
            masterClient.Connect(ModbusMasterFixture.DefaultModbusIPEndPoint);
            using (var master = ModbusIpMaster.CreateIp(masterClient))
            {
                master.Transport.Retries = 0;

                var random = new Random();
                for (int i = 0; i < 5; i++)
                {
                    bool[] coils = master.ReadCoils(1, 1);
                    Assert.Equal(1, coils.Length);
                    Debug.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: Reading coil value");
                    await Task.Delay(random.Next(100)).ConfigureAwait(false);
                }
            }
        }

        private Task CreateAndStartUdpSlave(int port, DataStore dataStore)
        {
            UdpClient slaveClient = new UdpClient(port);
            ModbusSlave slave = ModbusUdpSlave.CreateUdp(slaveClient);
            slave.DataStore = dataStore;

            return Task.Factory.StartNew(
                x => ((ModbusSlave)x).Listen(),
                slave,
                CancellationToken.None,
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
        }
    }
}
