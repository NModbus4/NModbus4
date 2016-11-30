using System;
using System.IO;
using System.Linq;
using Modbus.Data;
using Modbus.IO;
using Modbus.Message;
using Modbus.Utility;
using Rhino.Mocks;
using Xunit;

namespace Modbus.UnitTests.IO
{

    public class ModbusRtuTransportFixture
    {
        [Fact]
        public void BuildMessageFrame()
        {
            byte[] message = {17, ModbusConstants.ReadCoils, 0, 19, 0, 37, 14, 132};
            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(ModbusConstants.ReadCoils, 17, 19, 37);
            Assert.Equal(message,
                new ModbusRtuTransport(MockRepository.GenerateStub<IStreamResource>()).BuildMessageFrame(request));
        }

        [Fact]
        public void ResponseBytesToReadCoils()
        {
            byte[] frameStart = {0x11, 0x01, 0x05, 0xCD, 0x6B, 0xB2, 0x0E, 0x1B};
            Assert.Equal(6, ModbusRtuTransport.ResponseBytesToRead(frameStart));
        }

        [Fact]
        public void ResponseBytesToReadCoilsNoData()
        {
            byte[] frameStart = {0x11, 0x01, 0x00, 0x00, 0x00};
            Assert.Equal(1, ModbusRtuTransport.ResponseBytesToRead(frameStart));
        }

        [Fact]
        public void ResponseBytesToReadWriteCoilsResponse()
        {
            byte[] frameStart = {0x11, 0x0F, 0x00, 0x13, 0x00, 0x0A, 0, 0};
            Assert.Equal(4, ModbusRtuTransport.ResponseBytesToRead(frameStart));
        }

        [Fact]
        public void ResponseBytesToReadDiagnostics()
        {
            byte[] frameStart = {0x01, 0x08, 0x00, 0x00};
            Assert.Equal(4, ModbusRtuTransport.ResponseBytesToRead(frameStart));
        }

        [Fact]
        public void ResponseBytesToReadSlaveException()
        {
            byte[] frameStart = {0x01, ModbusConstants.ExceptionOffset + 1, 0x01};
            Assert.Equal(1, ModbusRtuTransport.ResponseBytesToRead(frameStart));
        }

        [Fact]
        public void ResponseBytesToReadInvalidFunctionCode()
        {
            byte[] frame = {0x11, 0x16, 0x00, 0x01, 0x00, 0x02, 0x04};
            Assert.Throws<NotImplementedException>(() => ModbusRtuTransport.ResponseBytesToRead(frame));
        }

        [Fact]
        public void RequestBytesToReadDiagnostics()
        {
            byte[] frame = {0x01, 0x08, 0x00, 0x00, 0xA5, 0x37, 0, 0};
            Assert.Equal(1, ModbusRtuTransport.RequestBytesToRead(frame));
        }

        [Fact]
        public void RequestBytesToReadCoils()
        {
            byte[] frameStart = {0x11, 0x01, 0x00, 0x13, 0x00, 0x25};
            Assert.Equal(1, ModbusRtuTransport.RequestBytesToRead(frameStart));
        }

        [Fact]
        public void RequestBytesToReadWriteCoilsRequest()
        {
            byte[] frameStart = {0x11, 0x0F, 0x00, 0x13, 0x00, 0x0A, 0x02, 0xCD, 0x01};
            Assert.Equal(4, ModbusRtuTransport.RequestBytesToRead(frameStart));
        }

        [Fact]
        public void RequestBytesToReadWriteMultipleHoldingRegisters()
        {
            byte[] frameStart = {0x11, 0x10, 0x00, 0x01, 0x00, 0x02, 0x04};
            Assert.Equal(6, ModbusRtuTransport.RequestBytesToRead(frameStart));
        }

        [Fact]
        public void RequestBytesToReadInvalidFunctionCode()
        {
            byte[] frame = {0x11, 0xFF, 0x00, 0x01, 0x00, 0x02, 0x04};
            Assert.Throws<NotImplementedException>(() => ModbusRtuTransport.RequestBytesToRead(frame));
        }

        [Fact]
        public void ChecksumsMatchSucceed()
        {
            ModbusRtuTransport transport = new ModbusRtuTransport(MockRepository.GenerateStub<IStreamResource>());
            ReadCoilsInputsRequest message = new ReadCoilsInputsRequest(ModbusConstants.ReadCoils, 17, 19, 37);
            byte[] frame = {17, ModbusConstants.ReadCoils, 0, 19, 0, 37, 14, 132};
            Assert.True(transport.ChecksumsMatch(message, frame));
        }

        [Fact]
        public void ChecksumsMatchFail()
        {
            ModbusRtuTransport transport = new ModbusRtuTransport(MockRepository.GenerateStub<IStreamResource>());
            ReadCoilsInputsRequest message = new ReadCoilsInputsRequest(ModbusConstants.ReadCoils, 17, 19, 38);
            byte[] frame = {17, ModbusConstants.ReadCoils, 0, 19, 0, 37, 14, 132};
            Assert.False(transport.ChecksumsMatch(message, frame));
        }

        [Fact]
        public void ReadResponse()
        {
            MockRepository mocks = new MockRepository();
            ModbusRtuTransport transport =
                mocks.PartialMock<ModbusRtuTransport>(MockRepository.GenerateStub<IStreamResource>());

            Expect.Call(transport.Read(ModbusRtuTransport.ResponseFrameStartLength))
                .Return(new byte[] {1, 1, 1, 0});

            Expect.Call(transport.Read(2))
                .Return(new byte[] {81, 136});

            mocks.ReplayAll();

            ReadCoilsInputsResponse response =
                transport.ReadResponse<ReadCoilsInputsResponse>() as ReadCoilsInputsResponse;
            Assert.NotNull(response);
            ReadCoilsInputsResponse expectedResponse = new ReadCoilsInputsResponse(ModbusConstants.ReadCoils, 1, 1,
                new DiscreteCollection(false));
            Assert.Equal(response.MessageFrame, expectedResponse.MessageFrame);

            mocks.VerifyAll();
        }

        [Fact]
        public void ReadResponseSlaveException()
        {
            MockRepository mocks = new MockRepository();
            ModbusRtuTransport transport =
                mocks.PartialMock<ModbusRtuTransport>(MockRepository.GenerateStub<IStreamResource>());

            byte[] messageFrame = {0x01, 0x81, 0x02};
            byte[] crc = ModbusUtility.CalculateCrc(messageFrame);

            Expect.Call(transport.Read(ModbusRtuTransport.ResponseFrameStartLength))
                .Return(Enumerable.Concat(messageFrame, new byte[] { crc[0] }).ToArray());

            Expect.Call(transport.Read(1))
                .Return(new byte[] {crc[1]});

            mocks.ReplayAll();

            Assert.True(transport.ReadResponse<ReadCoilsInputsResponse>() is SlaveExceptionResponse);

            mocks.VerifyAll();
        }

        /// <summary>
        ///     We want to throw an IOException for any message w/ an invalid checksum,
        ///     this must preceed throwing a SlaveException based on function code > 127
        /// </summary>
        [Fact]
        public void ReadResponseSlaveExceptionWithErroneousLrc()
        {
            MockRepository mocks = new MockRepository();
            ModbusRtuTransport transport =
                mocks.PartialMock<ModbusRtuTransport>(MockRepository.GenerateStub<IStreamResource>());

            byte[] messageFrame = {0x01, 0x81, 0x02};
            // invalid crc
            byte[] crc = {0x9, 0x9};

            Expect.Call(transport.Read(ModbusRtuTransport.ResponseFrameStartLength))
                .Return(Enumerable.Concat(messageFrame, new byte[] { crc[0] }).ToArray());

            Expect.Call(transport.Read(1))
                .Return(new byte[] {crc[1]});

            mocks.ReplayAll();

            Assert.Throws<IOException>(() => transport.ReadResponse<ReadCoilsInputsResponse>());

            mocks.VerifyAll();
        }

        [Fact]
        public void ReadRequest()
        {
            MockRepository mocks = new MockRepository();
            ModbusRtuTransport transport =
                mocks.PartialMock<ModbusRtuTransport>(MockRepository.GenerateStub<IStreamResource>());

            Expect.Call(transport.Read(ModbusRtuTransport.RequestFrameStartLength))
                .Return(new byte[] {1, 1, 1, 0, 1, 0, 0});

            Expect.Call(transport.Read(1))
                .Return(new byte[] {5});

            mocks.ReplayAll();

            Assert.Equal(new byte[] {1, 1, 1, 0, 1, 0, 0, 5}, transport.ReadRequest());
            mocks.VerifyAll();
        }

        [Fact]
        public void Read()
        {
            MockRepository mocks = new MockRepository();
            IStreamResource mockSerialResource = mocks.StrictMock<IStreamResource>();

            Expect.Call(mockSerialResource.Read(new byte[5], 0, 5))
                .Do(((Func<byte[], int, int, int>) delegate(byte[] buf, int offset, int count)
               {
                   Array.Copy(new byte[] {2, 2, 2}, buf, 3);
                   return 3;
               }));

            Expect.Call(mockSerialResource.Read(new byte[] {2, 2, 2, 0, 0}, 3, 2))
                .Do(((Func<byte[], int, int, int>) delegate(byte[] buf, int offset, int count)
               {
                   Array.Copy(new byte[] {3, 3}, 0, buf, 3, 2);
                   return 2;
               }));

            mocks.ReplayAll();

            ModbusRtuTransport transport = new ModbusRtuTransport(mockSerialResource);
            Assert.Equal(new byte[] {2, 2, 2, 3, 3}, transport.Read(5));

            mocks.VerifyAll();
        }
    }
}