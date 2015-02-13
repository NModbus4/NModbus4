using System;
using Modbus.Utility;

namespace Modbus.UnitTests.Utility
{
    using NUnit.Framework;

    [TestFixture]
    public class DiscriminatedUnionTestSuite
    {
        [Test]
        public void DiscriminatedUnion_CreateA()
        {
            var du = DiscriminatedUnion<string, string>.CreateA("foo");
            Assert.AreEqual(DiscriminatedUnionOption.A, du.Option);
            Assert.AreEqual("foo", du.A);
        }

        [Test]
        public void DiscriminatedUnion_CreateB()
        {
            var du = DiscriminatedUnion<string, string>.CreateB("foo");
            Assert.AreEqual(DiscriminatedUnionOption.B, du.Option);
            Assert.AreEqual("foo", du.B);
        }

        [Test]
        public void DiscriminatedUnion_AllowNulls()
        {
            var du = DiscriminatedUnion<object, object>.CreateB(null);
            Assert.AreEqual(DiscriminatedUnionOption.B, du.Option);
            Assert.AreEqual(null, du.B);
        }

        [Test, ExpectedException(typeof (InvalidOperationException))]
        public void AccessInvalidOption_A()
        {
            var du = DiscriminatedUnion<string, string>.CreateB("foo");
            du.A.ToString();
        }

        [Test, ExpectedException(typeof (InvalidOperationException))]
        public void AccessInvalidOption_B()
        {
            var du = DiscriminatedUnion<string, string>.CreateA("foo");
            du.B.ToString();
        }

        [Test]
        public void DiscriminatedUnion_ToString()
        {
            var du = DiscriminatedUnion<string, string>.CreateA("foo");
            Assert.AreEqual(du.ToString(), "foo");
        }
    }
}