using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Modbus.Data;
using Modbus.IO;
using Modbus.Message;
using Modbus.UnitTests.Message;
using Rhino.Mocks;
using Xunit;

namespace Modbus.UnitTests.IO
{
    public class ModbusTcpTransportFixture
    {
        [Fact]
        public void BuildMessageFrame()
        {
            MockRepository mocks = new MockRepository();
            ModbusIpTransport mockModbusTcpTransport =
                mocks.PartialMock<ModbusIpTransport>(MockRepository.GenerateStub<IStreamResource>());
            ReadCoilsInputsRequest message = new ReadCoilsInputsRequest(Modbus.ReadCoils, 2, 10, 5);
            mocks.ReplayAll();
            byte[] result = mockModbusTcpTransport.BuildMessageFrame(message);
            Assert.Equal(new byte[] {0, 0, 0, 0, 0, 6, 2, 1, 0, 10, 0, 5}, result);
            mocks.VerifyAll();
        }

        [Fact]
        public void GetMbapHeader()
        {
            WriteMultipleRegistersRequest message = new WriteMultipleRegistersRequest(3, 1,
                MessageUtility.CreateDefaultCollection<RegisterCollection, ushort>(0, 120));
            message.TransactionId = 45;
            Assert.Equal(new byte[] {0, 45, 0, 0, 0, 247, 3}, ModbusIpTransport.GetMbapHeader(message));
        }

        [Fact]
        public void Write()
        {
            MockRepository mocks = new MockRepository();
            var mockTcpStreamAdapter = mocks.StrictMock<IStreamResource>();
            var bytesToWrite = new byte[] {255, 255, 0, 0, 0, 6, 1, 1, 0, 1, 0, 3};
            mockTcpStreamAdapter.Write(bytesToWrite, 0, bytesToWrite.Length);
            ModbusIpTransport mockModbusTcpTransport = mocks.PartialMock<ModbusIpTransport>(mockTcpStreamAdapter);
            Expect.Call(mockModbusTcpTransport.GetNewTransactionId()).Return(UInt16.MaxValue);
            mocks.ReplayAll();
            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 3);
            mockModbusTcpTransport.Write(request);
            mocks.VerifyAll();
        }

        [Fact]
        public void ReadRequestResponse()
        {
            MockRepository mocks = new MockRepository();
            var mockTransport = mocks.StrictMock<IStreamResource>(null);

            byte[] mbapHeader = {45, 63, 0, 0, 0, 6};

            Expect.Call(mockTransport.Read(new byte[6], 0, 6))
                .Do(((Func<byte[], int, int, int>) delegate(byte[] buf, int offset, int count)
               {
                   Array.Copy(mbapHeader, buf, 6);
                   return 6;
               }));

            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 3);

            Expect.Call(mockTransport.Read(new byte[6], 0, 6))
                .Do(((Func<byte[], int, int, int>) delegate(byte[] buf, int offset, int count)
               {
                   Array.Copy(new byte[] {1}.Concat(request.ProtocolDataUnit).ToArray(), buf, 6);
                   return 6;
               }));

            mocks.ReplayAll();
            Assert.Equal(ModbusIpTransport.ReadRequestResponse(mockTransport),
                new byte[] {45, 63, 0, 0, 0, 6, 1, 1, 0, 1, 0, 3});
            mocks.VerifyAll();
        }

        [Fact]
        public void ReadRequestResponse_ConnectionAbortedWhileReadingMBAPHeader()
        {
            MockRepository mocks = new MockRepository();
            var mockTransport = mocks.StrictMock<IStreamResource>(null);

            Expect.Call(mockTransport.Read(new byte[6], 0, 6)).Return(0);

            mocks.ReplayAll();
            Assert.Throws<IOException>(() => ModbusIpTransport.ReadRequestResponse(mockTransport));
            mocks.VerifyAll();
        }

        [Fact]
        public void ReadRequestResponse_ConnectionAbortedWhileReadingMessageFrame()
        {
            MockRepository mocks = new MockRepository();
            var mockTransport = mocks.StrictMock<IStreamResource>(null);

            byte[] mbapHeader = {45, 63, 0, 0, 0, 6};

            Expect.Call(mockTransport.Read(new byte[6], 0, 6))
                .Do(((Func<byte[], int, int, int>) delegate(byte[] buf, int offset, int count)
               {
                   Array.Copy(mbapHeader, buf, 6);
                   return 6;
               }));

            Expect.Call(mockTransport.Read(new byte[6], 0, 6)).Return(0);

            mocks.ReplayAll();
            Assert.Throws<IOException>(() => ModbusIpTransport.ReadRequestResponse(mockTransport));
            mocks.VerifyAll();
        }

        [Fact]
        public void GetNewTransactionId()
        {
            ModbusIpTransport transport = new ModbusIpTransport(MockRepository.GenerateStub<IStreamResource>());
            Dictionary<int, string> transactionIds = new Dictionary<int, string>();

            for (int i = 0; i < UInt16.MaxValue; i++)
                transactionIds.Add(transport.GetNewTransactionId(), String.Empty);

            Assert.Equal(1, transport.GetNewTransactionId());
            Assert.Equal(2, transport.GetNewTransactionId());
        }

        [Fact]
        public void OnShouldRetryResponse_ReturnsTrue_IfWithinThreshold()
        {
            ModbusIpTransport transport = new ModbusIpTransport(MockRepository.GenerateStub<IStreamResource>());
            IModbusMessage request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            IModbusMessage response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, null);

            request.TransactionId = 5;
            response.TransactionId = 4;
            transport.RetryOnOldResponseThreshold = 3;

            Assert.True(transport.OnShouldRetryResponse(request, response));
        }

        [Fact]
        public void OnShouldRetryResponse_ReturnsFalse_IfThresholdDisabled()
        {
            ModbusIpTransport transport = new ModbusIpTransport(MockRepository.GenerateStub<IStreamResource>());
            IModbusMessage request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            IModbusMessage response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, null);

            request.TransactionId = 5;
            response.TransactionId = 4;
            transport.RetryOnOldResponseThreshold = 0;

            Assert.False(transport.OnShouldRetryResponse(request, response));
        }

        [Fact]
        public void OnShouldRetryResponse_ReturnsFalse_IfEqualTransactionId()
        {
            ModbusIpTransport transport = new ModbusIpTransport(MockRepository.GenerateStub<IStreamResource>());
            IModbusMessage request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            IModbusMessage response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, null);

            request.TransactionId = 5;
            response.TransactionId = 5;
            transport.RetryOnOldResponseThreshold = 3;

            Assert.False(transport.OnShouldRetryResponse(request, response));
        }

        [Fact]
        public void OnShouldRetryResponse_ReturnsFalse_IfOutsideThreshold()
        {
            ModbusIpTransport transport = new ModbusIpTransport(MockRepository.GenerateStub<IStreamResource>());
            IModbusMessage request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            IModbusMessage response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, null);

            request.TransactionId = 5;
            response.TransactionId = 2;
            transport.RetryOnOldResponseThreshold = 3;

            Assert.False(transport.OnShouldRetryResponse(request, response));
        }

        [Fact]
        public void ValidateResponse_MismatchingTransactionIds()
        {
            ModbusIpTransport transport = new ModbusIpTransport(MockRepository.GenerateStub<IStreamResource>());

            IModbusMessage request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            request.TransactionId = 5;
            IModbusMessage response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, null);
            response.TransactionId = 6;

            Assert.Throws<IOException>(() => transport.ValidateResponse(request, response));
        }

        [Fact]
        public void ValidateResponse()
        {
            ModbusIpTransport transport = new ModbusIpTransport(MockRepository.GenerateStub<IStreamResource>());

            IModbusMessage request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            request.TransactionId = 5;
            IModbusMessage response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, null);
            response.TransactionId = 5;

            // no exception is thrown
            transport.ValidateResponse(request, response);
        }
    }
}