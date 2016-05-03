using Kaleidoscope.Primitive;
using Kaleidoscope.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestTokenizer
{
	[TestClass]
	public class TestHexNumber
	{
		[TestMethod]
		public void TestHexDefault1()
		{
			const string Content = "0xFF_FF_FF_FF";
			var token = Util.Process<TokenUnsignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 0xFFFFFFFF);
		}

		[TestMethod]
		public void TestHexDefault2()
		{
			const string Content = "0x0F'FF_FF'FF";
			var token = Util.Process<TokenSignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 0x0FFFFFFF);
		}

		[TestMethod]
		public void TestHexUL1()
		{
			const string Content = "0xFFFFFFFFUL";
			var token = Util.Process<TokenUnsignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 0xFFFFFFFFUL);
		}

		[TestMethod]
		public void TestHexUL2()
		{
			const string Content = "0xFFFF'FFFF'FFFF'FFFFL";
			var token = Util.Process<TokenUnsignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 0xFFFFFFFFFFFFFFFF);
		}

		[TestMethod]
		public void TestHexL1()
		{
			const string Content = "0xFFFF_FFFFL";
			var token = Util.Process<TokenSignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 0xFFFFFFFFL);
		}

		[TestMethod]
		public void TestHexL2()
		{
			const string Content = "0xFFF'FFFF'FFFF'FFFF";
			var token = Util.Process<TokenSignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 0xFFFFFFFFFFFFFFF);
		}

		[TestMethod]
		public void TestHexU1()
		{
			const string Content = "0xFFF_FFFU";
			var token = Util.Process<TokenUnsignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 0xFFFFFF);
		}

		[TestMethod]
		public void TestHexU2()
		{
			const string Content = "0xFFF'FFFF'FFFF'FFFFU";
			var token = Util.Process<TokenUnsignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 0xFFFFFFFFFFFFFFF);
		}
	}
}
