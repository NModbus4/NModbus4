using Modbus.Data;
using Modbus.Message;

namespace Modbus.UnitTests.Message
{
    using NUnit.Framework;

    [TestFixture]
    public class ReturnQueryDataRequestResponseFixture
    {
        [Test]
        public void ReturnQueryDataRequestResponse()
        {
            RegisterCollection data = new RegisterCollection(1, 2, 3, 4);
            DiagnosticsRequestResponse request = new DiagnosticsRequestResponse(Modbus.DiagnosticsReturnQueryData, 5,
                data);
            Assert.AreEqual(Modbus.Diagnostics, request.FunctionCode);
            Assert.AreEqual(Modbus.DiagnosticsReturnQueryData, request.SubFunctionCode);
            Assert.AreEqual(5, request.SlaveAddress);
            Assert.AreEqual(data.NetworkBytes, request.Data.NetworkBytes);
        }

        [Test]
        public void ProtocolDataUnit()
        {
            RegisterCollection data = new RegisterCollection(1, 2, 3, 4);
            DiagnosticsRequestResponse request = new DiagnosticsRequestResponse(Modbus.DiagnosticsReturnQueryData, 5,
                data);
            Assert.AreEqual(new byte[] {8, 0, 0, 0, 1, 0, 2, 0, 3, 0, 4}, request.ProtocolDataUnit);
        }
    }
}