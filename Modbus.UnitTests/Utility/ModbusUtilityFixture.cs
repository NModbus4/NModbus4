using System;
using Modbus.Message;
using Modbus.Utility;

namespace Modbus.UnitTests.Utility
{
    using NUnit.Framework;

    [TestFixture]
	public class ModbusUtilityFixture
	{
		[Test]
		public void GetAsciiBytesFromEmpty()
		{
			Assert.AreEqual(new byte[] { }, ModbusUtility.GetAsciiBytes(new byte[] { }));
			Assert.AreEqual(new byte[] { }, ModbusUtility.GetAsciiBytes(new ushort[] { }));
		}

		[Test]
		public void GetAsciiBytesFromBytes()
		{
			byte[] buf = { 2, 5 };
			byte[] expectedResult = { 48, 50, 48, 53 };
			byte[] result = ModbusUtility.GetAsciiBytes(buf);
			Assert.AreEqual(expectedResult, result);
		}

		[Test]
		public void GetAsciiBytesFromUshorts()
		{
			ushort[] buf = { 300, 400 };
			byte[] expectedResult = { 48, 49, 50, 67, 48, 49, 57, 48 };
			byte[] result = ModbusUtility.GetAsciiBytes(buf);
			Assert.AreEqual(expectedResult, result);			
		}

		[Test]
		public void HexToBytes()
		{
			Assert.AreEqual(new byte[] { 255 }, ModbusUtility.HexToBytes("FF"));	
		}

		[Test]
		public void HexToBytes2()
		{
			Assert.AreEqual(new byte[] { 204, 255}, ModbusUtility.HexToBytes("CCFF"));
		}

		[Test]
		public void HexToBytesEmpty()
		{
			Assert.AreEqual(new byte[] { }, ModbusUtility.HexToBytes(""));			
		}
		
		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void HexToBytesNull()
		{
			ModbusUtility.HexToBytes(null);
			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void HexToBytesOdd()
		{
			ModbusUtility.HexToBytes("CCF");
			Assert.Fail();
		}

		[Test]
		public void CalculateCrc()
		{
			byte[] result = ModbusUtility.CalculateCrc(new byte[] { 1, 1 });
			Assert.AreEqual(new byte[] { 193, 224 }, result);			
		}

		[Test]
		public void CalculateCrc2()
		{
			byte[] result = ModbusUtility.CalculateCrc(new byte[] { 2, 1, 5, 0 });
			Assert.AreEqual(new byte[] { 83, 12 }, result);
		}

		[Test]
		public void CalculateCrcEmpty()
		{
			Assert.AreEqual(new byte[] { 255, 255 }, ModbusUtility.CalculateCrc(new byte[] { }));
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void CalculateCrcNull()
		{
			ModbusUtility.CalculateCrc(null);
			Assert.Fail();
		}

		[Test]
		public void CalculateLrc()
		{
			ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 10);
			Assert.AreEqual(243, ModbusUtility.CalculateLrc(new byte[] { 1, 1, 0, 1, 0, 10 }));
		}

		[Test]
		public void CalculateLrc2()
		{
			//: 02 01 0000 0001 FC
			ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 2, 0, 1);			
			Assert.AreEqual(252, ModbusUtility.CalculateLrc(new byte[] { 2, 1, 0, 0, 0, 1}));
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void CalculateLrcNull()
		{
			ModbusUtility.CalculateLrc(null);
			Assert.Fail();
		}

		[Test]
		public void CalculateLrcEmpty()
		{
			Assert.AreEqual(0, ModbusUtility.CalculateLrc(new byte[] {}));
		}

		[Test]
		public void NetworkBytesToHostUInt16()
		{
			Assert.AreEqual(new ushort[] { 1, 2 }, ModbusUtility.NetworkBytesToHostUInt16(new byte[] { 0, 1, 0, 2 }));
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void NetworkBytesToHostUInt16Null()
		{
			ModbusUtility.NetworkBytesToHostUInt16(null);
			Assert.Fail();
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void NetworkBytesToHostUInt16OddNumberOfBytes()
		{
			ModbusUtility.NetworkBytesToHostUInt16(new byte[] { 1 });
			Assert.Fail();
		}

		[Test]
		public void NetworkBytesToHostUInt16EmptyBytes()
		{
			Assert.AreEqual(new ushort[] { }, ModbusUtility.NetworkBytesToHostUInt16(new byte[] { }));
		}

		[Test]
		public void GetSingle()
		{
			Assert.AreEqual(0F, ModbusUtility.GetSingle(0, 0));
			Assert.AreEqual(1F, ModbusUtility.GetSingle(16256, 0));
			Assert.AreEqual(9999999F, ModbusUtility.GetSingle(19224, 38527));
			Assert.AreEqual(500.625F, ModbusUtility.GetSingle(17402, 20480));
		}

		[Test]
		public void GetUInt32()
		{
			Assert.AreEqual(0, ModbusUtility.GetUInt32(0, 0));
			Assert.AreEqual(1, ModbusUtility.GetUInt32(0, 1));
			Assert.AreEqual(45, ModbusUtility.GetUInt32(0, 45));
			Assert.AreEqual(65536, ModbusUtility.GetUInt32(1, 0));
		}
	}
}