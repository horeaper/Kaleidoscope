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
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenSignedInteger;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 128);
		}

		[TestMethod]
		public void TestBinaryUL()
		{
			const string Content = "0b10'00'00'00UL";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenUnsignedInteger;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 128);
		}

		[TestMethod]
		public void TestBinaryU()
		{
			const string Content = "0b0001_0000_0000U";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenUnsignedInteger;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 256);
		}

		[TestMethod]
		public void TestBinaryL()
		{
			const string Content = "0b1000_0000_0000L";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenSignedInteger;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 2048);
		}
	}
}
