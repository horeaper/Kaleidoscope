using Kaleidoscope.Primitive;
using Kaleidoscope.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestTokenizer
{
	[TestClass]
	public class TestBinaryNumber
	{
		[TestMethod]
		public void TestBinaryDefault()
		{
			const string Content = "0b10'00'00'00";
			var token = Util.Process<TokenSignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 128);
		}

		[TestMethod]
		public void TestBinaryUL()
		{
			const string Content = "0b10'00'00'00UL";
			var token = Util.Process<TokenUnsignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 128);
		}

		[TestMethod]
		public void TestBinaryU()
		{
			const string Content = "0b0001_0000_0000U";
			var token = Util.Process<TokenUnsignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 256);
		}

		[TestMethod]
		public void TestBinaryL()
		{
			const string Content = "0b1000_0000_0000L";
			var token = Util.Process<TokenSignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 2048);
		}
	}
}
