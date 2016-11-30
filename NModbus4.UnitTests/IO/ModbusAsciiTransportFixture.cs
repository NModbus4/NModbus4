using System;
using System.IO;
using System.Text;
using Modbus.IO;
using Modbus.Message;
using Modbus.UnitTests.Message;
using Rhino.Mocks;
using Xunit;

namespace Modbus.UnitTests.IO
{

    public class ModbusAsciiTransportFixture : ModbusMessageFixture
    {
        [Fact]
        public void BuildMessageFrame()
        {
            byte[] expected = {58, 48, 50, 48, 49, 48, 48, 48, 48, 48, 48, 48, 49, 70, 67, 13, 10};
            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(ModbusConstants.ReadCoils, 2, 0, 1);
            var actual =
                new ModbusAsciiTransport(MockRepository.GenerateStub<IStreamResource>()).BuildMessageFrame(request);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReadRequestResponse()
        {
            var mocks = new MockRepository();
            var stream = mocks.StrictMock<IStreamResource>();
            var transport = new ModbusAsciiTransport(stream);

            ExpectReadLine(stream, Encoding.ASCII.GetBytes(":110100130025B6\r\n"));

            mocks.ReplayAll();

            Assert.Equal(new byte[] {17, 1, 0, 19, 0, 37, 182}, transport.ReadRequestResponse());

            mocks.VerifyAll();
        }

        [Fact]
        public void ReadRequestResponseNotEnoughBytes()
        {
            MockRepository mocks = new MockRepository();
            IStreamResource mockSerialResource = mocks.StrictMock<IStreamResource>();

            var mockTransport = mocks.PartialMock<ModbusAsciiTransport>(mockSerialResource);
            ExpectReadLine(mockSerialResource, Encoding.ASCII.GetBytes(":10\r\n"));
            mocks.ReplayAll();

            Assert.Throws<IOException>(() => mockTransport.ReadRequestResponse());

            mocks.VerifyAll();
        }

        [Fact]
        public void ChecksumsMatchSucceed()
        {
            ModbusAsciiTransport transport = new ModbusAsciiTransport(MockRepository.GenerateStub<IStreamResource>());
            ReadCoilsInputsRequest message = new ReadCoilsInputsRequest(ModbusConstants.ReadCoils, 17, 19, 37);
            byte[] frame = {17, ModbusConstants.ReadCoils, 0, 19, 0, 37, 182};
            Assert.True(transport.ChecksumsMatch(message, frame));
        }

        [Fact]
        public void ChecksumsMatchFail()
        {
            ModbusAsciiTransport transport = new ModbusAsciiTransport(MockRepository.GenerateStub<IStreamResource>());
            ReadCoilsInputsRequest message = new ReadCoilsInputsRequest(ModbusConstants.ReadCoils, 17, 19, 37);
            byte[] frame = {17, ModbusConstants.ReadCoils, 0, 19, 0, 37, 181};
            Assert.False(transport.ChecksumsMatch(message, frame));
        }

        private static void ExpectReadLine(IStreamResource stream, byte[] frame)
        {
            byte lastByte = 0;

            foreach (var b in frame)
            {
                byte tempByte = b;
                Expect.Call(stream.Read(new byte[] {lastByte}, 0, 1))
                    .Do(((Func<byte[], int, int, int>) delegate(byte[] buf, int offset, int count)
                   {
                       Array.Copy(new byte[] {tempByte}, buf, 1);
                       return 1;
                   }));

                lastByte = tempByte;
            }
        }

        private class TestSerialPortAdapter : IStreamResource
        {
            public int InfiniteTimeout
            {
                get { throw new NotImplementedException(); }
            }

            public int ReadTimeout
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public int WriteTimeout
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public void DiscardInBuffer()
            {
                throw new NotImplementedException();
            }

            public int Read(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }

            public void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }

            public string ReadLine()
            {
                return "FooBar";
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
    }
}