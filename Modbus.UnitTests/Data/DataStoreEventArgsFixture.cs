using System.Linq;
using Modbus.Data;

namespace Modbus.UnitTests.Data
{
    using System;

    using NUnit.Framework;

    [TestFixture]
	public class DataStoreEventArgsFixture
	{
		[Test]
		public void CreateDataStoreEventArgs()
		{
			var eventArgs = DataStoreEventArgs.CreateDataStoreEventArgs(5, ModbusDataType.HoldingRegister, new ushort[] { 1, 2, 3 });
			Assert.AreEqual(ModbusDataType.HoldingRegister, eventArgs.ModbusDataType);
			Assert.AreEqual(5, eventArgs.StartAddress);
			Assert.AreEqual(new ushort[] { 1, 2, 3 }, eventArgs.Data.B.ToArray());
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void CreateDataStoreEventArgs_InvalidType()
		{
			var eventArgs = DataStoreEventArgs.CreateDataStoreEventArgs(5, ModbusDataType.HoldingRegister, new int[] { 1, 2, 3 });
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void CreateDataStoreEventArgs_DataNull()
		{
			var eventArgs = DataStoreEventArgs.CreateDataStoreEventArgs(5, ModbusDataType.HoldingRegister, default(ushort[]));
		}
	}
}
