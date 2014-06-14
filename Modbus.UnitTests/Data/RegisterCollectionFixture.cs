using Modbus.Data;

namespace Modbus.UnitTests.Data
{
    using NUnit.Framework;

    [TestFixture]
	public class RegisterCollectionFixture
	{
		[Test]
		public void ByteCount()
		{
			RegisterCollection col = new RegisterCollection(1, 2, 3);
			Assert.AreEqual(6, col.ByteCount);
		}

		[Test]
		public void NewRegisterCollection()
		{
			RegisterCollection col = new RegisterCollection(5, 3, 4, 6);
			Assert.IsNotNull(col);
			Assert.AreEqual(4, col.Count);
			Assert.AreEqual(5, col[0]);
		}

		[Test]
		public void NewRegisterCollectionFromBytes()
		{
			RegisterCollection col = new RegisterCollection(new byte[] { 0, 1, 0, 2, 0, 3 });
			Assert.IsNotNull(col);
			Assert.AreEqual(3, col.Count);
			Assert.AreEqual(1, col[0]);
			Assert.AreEqual(2, col[1]);
			Assert.AreEqual(3, col[2]);
		}

		[Test]
		public void RegisterCollectionNetworkBytes()
		{
			RegisterCollection col = new RegisterCollection(5, 3, 4, 6);
			byte[] bytes = col.NetworkBytes;
			Assert.IsNotNull(bytes);
			Assert.AreEqual(8, bytes.Length);
			Assert.AreEqual(new byte[] { 0, 5, 0, 3, 0, 4, 0, 6 }, bytes);
		}

		[Test]
		public void RegisterCollectionEmpty()
		{
			RegisterCollection col = new RegisterCollection();
			Assert.IsNotNull(col);
			Assert.AreEqual(0, col.NetworkBytes.Length);
		}

		[Test]
		public void ModifyRegister()
		{
			RegisterCollection col = new RegisterCollection(1, 2, 3, 4);
			col[0] = 5;
		}

		[Test]
		public void AddRegister()
		{
			RegisterCollection col = new RegisterCollection();
			Assert.AreEqual(0, col.Count);
			col.Add(45);
			Assert.AreEqual(1, col.Count);
		}

		[Test]
		public void RemoveRegister()
		{
			RegisterCollection col = new RegisterCollection(3, 4, 5);
			Assert.AreEqual(3, col.Count);
			col.RemoveAt(2);
			Assert.AreEqual(2, col.Count);
		}
	}
}
