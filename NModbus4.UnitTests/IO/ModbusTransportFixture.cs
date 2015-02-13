using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Modbus.Data;
using Modbus.IO;
using Modbus.Message;
using Modbus.Utility;
using Rhino.Mocks;

namespace Modbus.UnitTests.IO
{
    using Unme.Common;
    using NUnit.Framework;

    internal delegate ReadCoilsInputsResponse ThrowExceptionDelegate();

    [TestFixture]
    public class ModbusTransportFixture
    {
        [Test]
        public void UnicastMessage()
        {
            MockRepository mocks = new MockRepository();
            ModbusTransport transport = mocks.PartialMock<ModbusTransport>();
            transport.Write(null);
            LastCall.IgnoreArguments();
            // read 4 coils from slave id 2
            Expect.Call(transport.ReadResponse<ReadCoilsInputsResponse>())
                .Return(new ReadCoilsInputsResponse(Modbus.ReadCoils, 2, 1,
                    new DiscreteCollection(true, false, true, false, false, false, false, false)));
            transport.OnValidateResponse(null, null);
            LastCall.IgnoreArguments();

            mocks.ReplayAll();

            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 2, 3, 4);
            ReadCoilsInputsResponse expectedResponse = new ReadCoilsInputsResponse(Modbus.ReadCoils, 2, 1,
                new DiscreteCollection(true, false, true, false, false, false, false, false));
            ReadCoilsInputsResponse response = transport.UnicastMessage<ReadCoilsInputsResponse>(request);
            Assert.AreEqual(expectedResponse.MessageFrame, response.MessageFrame);

            mocks.VerifyAll();
        }

        [Test, ExpectedException(typeof (IOException))]
        public void UnicastMessage_WrongResponseFunctionCode()
        {
            MockRepository mocks = new MockRepository();
            ModbusTransport transport = mocks.PartialMock<ModbusTransport>();
            transport.Write(null);
            LastCall.IgnoreArguments().Repeat.Times(Modbus.DefaultRetries + 1);
            // read 4 coils from slave id 2
            Expect.Call(transport.ReadResponse<ReadCoilsInputsResponse>())
                .Return(new ReadCoilsInputsResponse(Modbus.ReadCoils, 2, 0, new DiscreteCollection()))
                .Repeat.Times(Modbus.DefaultRetries + 1);

            mocks.ReplayAll();

            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadInputs, 2, 3, 4);
            transport.UnicastMessage<ReadCoilsInputsResponse>(request);

            mocks.VerifyAll();
        }

        [Test, ExpectedException(typeof (SlaveException))]
        public void UnicastMessage_ErrorSlaveException()
        {
            MockRepository mocks = new MockRepository();
            ModbusTransport transport = mocks.PartialMock<ModbusTransport>();
            transport.Write(null);
            LastCall.IgnoreArguments().Repeat.Times(Modbus.DefaultRetries + 1);
            Expect.Call(transport.ReadResponse<ReadCoilsInputsResponse>())
                .Do((ThrowExceptionDelegate) delegate { throw new SlaveException(); });

            mocks.ReplayAll();

            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadInputs, 2, 3, 4);
            transport.UnicastMessage<ReadCoilsInputsResponse>(request);

            mocks.VerifyAll();
        }

        /// <summary>
        ///     We should reread the response w/o retransmitting the request.
        /// </summary>
        [Test]
        public void UnicastMessage_AcknowlegeSlaveException()
        {
            MockRepository mocks = new MockRepository();
            ModbusTransport transport = mocks.PartialMock<ModbusTransport>();

            // set the wait to retry property to a small value so the test completes quickly
            transport.WaitToRetryMilliseconds = 5;

            transport.Write(null);
            LastCall.IgnoreArguments();

            // return a slave exception a greater number of times than number of retries to make sure we aren't just retrying
            Expect.Call(transport.ReadResponse<ReadHoldingInputRegistersResponse>())
                .Return(new SlaveExceptionResponse(1, Modbus.ReadHoldingRegisters + Modbus.ExceptionOffset,
                    Modbus.Acknowledge))
                .Repeat.Times(transport.Retries + 1);

            Expect.Call(transport.ReadResponse<ReadHoldingInputRegistersResponse>())
                .Return(new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1)));

            transport.OnValidateResponse(null, null);
            LastCall.IgnoreArguments();

            mocks.ReplayAll();

            ReadHoldingInputRegistersRequest request = new ReadHoldingInputRegistersRequest(
                Modbus.ReadHoldingRegisters, 1, 1, 1);
            ReadHoldingInputRegistersResponse expectedResponse =
                new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1));
            ReadHoldingInputRegistersResponse response =
                transport.UnicastMessage<ReadHoldingInputRegistersResponse>(request);
            Assert.AreEqual(expectedResponse.MessageFrame, response.MessageFrame);

            mocks.VerifyAll();
        }

        /// <summary>
        ///     We should retransmit the request.
        /// </summary>
        [Test]
        public void UnicastMessage_SlaveDeviceBusySlaveException()
        {
            MockRepository mocks = new MockRepository();
            ModbusTransport transport = mocks.PartialMock<ModbusTransport>();

            // set the wait to retry property to a small value so the test completes quickly
            transport.WaitToRetryMilliseconds = 5;

            transport.Write(null);
            LastCall.IgnoreArguments()
                .Repeat.Times(2);

            // return a slave exception a greater number of times than number of retries to make sure we aren't just retrying
            Expect.Call(transport.ReadResponse<ReadHoldingInputRegistersResponse>())
                .Return(new SlaveExceptionResponse(1, Modbus.ReadHoldingRegisters + Modbus.ExceptionOffset,
                    Modbus.SlaveDeviceBusy));

            Expect.Call(transport.ReadResponse<ReadHoldingInputRegistersResponse>())
                .Return(new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1)));

            transport.OnValidateResponse(null, null);
            LastCall.IgnoreArguments();

            mocks.ReplayAll();

            ReadHoldingInputRegistersRequest request = new ReadHoldingInputRegistersRequest(
                Modbus.ReadHoldingRegisters, 1, 1, 1);
            ReadHoldingInputRegistersResponse expectedResponse =
                new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1));
            ReadHoldingInputRegistersResponse response =
                transport.UnicastMessage<ReadHoldingInputRegistersResponse>(request);
            Assert.AreEqual(expectedResponse.MessageFrame, response.MessageFrame);

            mocks.VerifyAll();
        }

        /// <summary>
        ///     We should retransmit the request.
        /// </summary>
        [Test]
        public void UnicastMessage_SlaveDeviceBusySlaveExceptionDoesNotFailAfterExceedingRetries()
        {
            MockRepository mocks = new MockRepository();
            ModbusTransport transport = mocks.PartialMock<ModbusTransport>();

            // set the wait to retry property to a small value so the test completes quickly
            transport.WaitToRetryMilliseconds = 5;

            transport.Write(null);
            LastCall.IgnoreArguments()
                .Repeat.Times(transport.Retries + 1);

            // return a slave exception a greater number of times than number of retries to make sure we aren't just retrying
            Expect.Call(transport.ReadResponse<ReadHoldingInputRegistersResponse>())
                .Return(new SlaveExceptionResponse(1, Modbus.ReadHoldingRegisters + Modbus.ExceptionOffset,
                    Modbus.SlaveDeviceBusy))
                .Repeat.Times(transport.Retries);

            Expect.Call(transport.ReadResponse<ReadHoldingInputRegistersResponse>())
                .Return(new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1)));

            transport.OnValidateResponse(null, null);
            LastCall.IgnoreArguments();

            mocks.ReplayAll();

            ReadHoldingInputRegistersRequest request = new ReadHoldingInputRegistersRequest(
                Modbus.ReadHoldingRegisters, 1, 1, 1);
            ReadHoldingInputRegistersResponse expectedResponse =
                new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1));
            ReadHoldingInputRegistersResponse response =
                transport.UnicastMessage<ReadHoldingInputRegistersResponse>(request);
            Assert.AreEqual(expectedResponse.MessageFrame, response.MessageFrame);

            mocks.VerifyAll();
        }

        [
            TestCase(typeof (TimeoutException)),
            TestCase(typeof (IOException)),
            TestCase(typeof (NotImplementedException)),
            TestCase(typeof (FormatException))]
        public void UnicastMessage_SingleFailingException(Type exceptionType)
        {
            MockRepository mocks = new MockRepository();
            ModbusTransport transport = mocks.PartialMock<ModbusTransport>();
            transport.Retries = 1;
            transport.Write(null);
            LastCall.IgnoreArguments().Repeat.Times(2);
            Expect.Call(transport.ReadResponse<ReadCoilsInputsResponse>())
                .Do((ThrowExceptionDelegate) delegate { throw (Exception) Activator.CreateInstance(exceptionType); });

            Expect.Call(transport.ReadResponse<ReadCoilsInputsResponse>())
                .Return(new ReadCoilsInputsResponse(Modbus.ReadCoils, 2, 1,
                    new DiscreteCollection(true, false, true, false, false, false, false, false)));

            transport.OnValidateResponse(null, null);
            LastCall.IgnoreArguments();

            mocks.ReplayAll();

            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 2, 3, 4);
            transport.UnicastMessage<ReadCoilsInputsResponse>(request);

            mocks.VerifyAll();
        }

        [TestCase(typeof (TimeoutException)),
         TestCase(typeof (IOException)),
         TestCase(typeof (NotImplementedException)),
         TestCase(typeof (FormatException))]
        public void UnicastMessage_TooManyFailingExceptions(Type exceptionType)
        {
            MockRepository mocks = new MockRepository();
            ModbusTransport transport = mocks.PartialMock<ModbusTransport>();

            transport.Write(null);
            LastCall.IgnoreArguments().Repeat.Times(transport.Retries + 1);

            Expect.Call(transport.ReadResponse<ReadCoilsInputsResponse>())
                .Do((ThrowExceptionDelegate) delegate { throw (Exception) Activator.CreateInstance(exceptionType); })
                .Repeat.Times(transport.Retries + 1);

            mocks.ReplayAll();

            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 2, 3, 4);

            Assert.Throws(exceptionType, () => transport.UnicastMessage<ReadCoilsInputsResponse>(request));

            mocks.VerifyAll();
        }

        [Test, ExpectedException(typeof (TimeoutException))]
        public void UnicastMessage_TimeoutException()
        {
            MockRepository mocks = new MockRepository();
            ModbusTransport transport = mocks.PartialMock<ModbusTransport>();
            transport.Write(null);
            LastCall.IgnoreArguments().Repeat.Times(Modbus.DefaultRetries + 1);
            Expect.Call(transport.ReadResponse<ReadCoilsInputsResponse>())
                .Do((ThrowExceptionDelegate) delegate { throw new TimeoutException(); })
                .Repeat.Times(Modbus.DefaultRetries + 1);

            mocks.ReplayAll();

            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadInputs, 2, 3, 4);
            transport.UnicastMessage<ReadCoilsInputsResponse>(request);

            mocks.VerifyAll();
        }

        [Test, ExpectedException(typeof (TimeoutException))]
        public void UnicastMessage_Retries()
        {
            MockRepository mocks = new MockRepository();
            ModbusTransport transport = mocks.PartialMock<ModbusTransport>();
            transport.Retries = 5;
            transport.Write(null);
            LastCall.IgnoreArguments().Repeat.Times(transport.Retries + 1);
            Expect.Call(transport.ReadResponse<ReadCoilsInputsResponse>())
                .Do((ThrowExceptionDelegate) delegate { throw new TimeoutException(); })
                .Repeat.Times(transport.Retries + 1);

            mocks.ReplayAll();

            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadInputs, 2, 3, 4);
            transport.UnicastMessage<ReadCoilsInputsResponse>(request);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateResponse_SlaveException()
        {
            ModbusTransport transport = new ModbusAsciiTransport(MockRepository.GenerateStub<IStreamResource>());
            byte[] frame = {2, 129, 2};
            byte lrc = ModbusUtility.CalculateLrc(frame);
            IModbusMessage message =
                transport.CreateResponse<ReadCoilsInputsResponse>(
                    Enumerable.Concat(frame, new byte[] { lrc }).ToArray());
            Assert.IsTrue(message is SlaveExceptionResponse);
        }

        [Test, ExpectedException(typeof (IOException))]
        public void ValidateResponse_MismatchingFunctionCodes()
        {
            MockRepository mocks = new MockRepository();
            ModbusTransport transport = mocks.PartialMock<ModbusTransport>();

            IModbusMessage request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            IModbusMessage response = new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1,
                new RegisterCollection());

            mocks.ReplayAll();
            transport.ValidateResponse(request, response);
            mocks.VerifyAll();
        }

        [Test]
        public void ValidateResponse()
        {
            MockRepository mocks = new MockRepository();
            ModbusTransport transport = mocks.PartialMock<ModbusTransport>();

            IModbusMessage request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            IModbusMessage response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, null);

            transport.OnValidateResponse(null, null);
            LastCall.IgnoreArguments();

            mocks.ReplayAll();
            transport.ValidateResponse(request, response);
            mocks.VerifyAll();
        }
    }
}