using System;
using System.IO;
using Modbus.Data;
using Modbus.IO;
using Modbus.Message;
using Modbus.UnitTests.Message;
using Modbus.Utility;
using Rhino.Mocks;
using Xunit;

namespace Modbus.UnitTests.IO
{
    public class ModbusSerialTransportFixture : ModbusMessageFixture
    {
        [Fact]
        public void CreateResponse()
        {
            ModbusAsciiTransport transport = new ModbusAsciiTransport(MockRepository.GenerateStub<IStreamResource>());
            ReadCoilsInputsResponse expectedResponse = new ReadCoilsInputsResponse(ModbusConstants.ReadCoils, 2, 1,
                new DiscreteCollection(true, false, false, false, false, false, false, true));
            byte lrc = ModbusUtility.CalculateLrc(expectedResponse.MessageFrame);
            ReadCoilsInputsResponse response =
                transport.CreateResponse<ReadCoilsInputsResponse>(new byte[] {2, ModbusConstants.ReadCoils, 1, 129, lrc}) as
                    ReadCoilsInputsResponse;
            Assert.NotNull(response);
            AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
        }

        [Fact]
        public void CreateResponseErroneousLrc()
        {
            ModbusAsciiTransport transport = new ModbusAsciiTransport(MockRepository.GenerateStub<IStreamResource>());
            transport.CheckFrame = true;
            Assert.Throws<IOException>(() => transport.CreateResponse<ReadCoilsInputsResponse>(new byte[] {19, ModbusConstants.ReadCoils, 0, 0, 0, 2, 115}));
        }

        [Fact]
        public void CreateResponseErroneousLrcDoNotCheckFrame()
        {
            ModbusAsciiTransport transport = new ModbusAsciiTransport(MockRepository.GenerateStub<IStreamResource>());
            transport.CheckFrame = false;
            transport.CreateResponse<ReadCoilsInputsResponse>(new byte[] {19, ModbusConstants.ReadCoils, 0, 0, 0, 2, 115});
        }

        /// <summary>
        ///     When using the serial RTU protocol the beginning of the message could get mangled leading to an unsupported message
        ///     type.
        ///     We want to be sure to try the message again so clear the RX buffer and try again.
        /// </summary>
        [Fact]
        public void UnicastMessage_PurgeReceiveBuffer()
        {
            MockRepository mocks = new MockRepository();
            IStreamResource serialResource = mocks.StrictMock<IStreamResource>();
            ModbusSerialTransport transport = new ModbusRtuTransport(serialResource);

            serialResource.DiscardInBuffer();
            serialResource.Write(null, 0, 0);
            LastCall.IgnoreArguments();

            // mangled response
            Expect.Call(serialResource.Read(new byte[] {0, 0, 0, 0}, 0, 4)).Return(4);

            serialResource.DiscardInBuffer();
            serialResource.Write(null, 0, 0);
            LastCall.IgnoreArguments();

            // normal response
            ReadCoilsInputsResponse response = new ReadCoilsInputsResponse(ModbusConstants.ReadCoils, 2, 1,
                new DiscreteCollection(true, false, true, false, false, false, false, false));

            // read header
            Expect.Call(serialResource.Read(new byte[] {0, 0, 0, 0}, 0, 4))
                .Do(((Func<byte[], int, int, int>) delegate(byte[] buf, int offset, int count)
               {
                   Array.Copy(response.MessageFrame, 0, buf, 0, 4);
                   return 4;
               }));

            // read remainder
            Expect.Call(serialResource.Read(new byte[] {0, 0}, 0, 2))
                .Do(((Func<byte[], int, int, int>) delegate(byte[] buf, int offset, int count)
               {
                   Array.Copy(ModbusUtility.CalculateCrc(response.MessageFrame), 0, buf, 0, 2);
                   return 2;
               }));

            mocks.ReplayAll();

            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(ModbusConstants.ReadCoils, 2, 3, 4);
            transport.UnicastMessage<ReadCoilsInputsResponse>(request);

            mocks.VerifyAll();
        }
    }
}