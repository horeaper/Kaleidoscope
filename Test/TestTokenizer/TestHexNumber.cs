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
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenUnsignedInteger;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 0xFFFFFFFF);
		}

		[TestMethod]
		public void TestHexDefault2()
		{
			const string Content = "0x0F'FF_FF'FF";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenSignedInteger;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 0x0FFFFFFF);
		}

		[TestMethod]
		public void TestHexUL1()
		{
			const string Content = "0xFFFFFFFFUL";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenUnsignedInteger;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 0xFFFFFFFFUL);
		}

		[TestMethod]
		public void TestHexUL2()
		{
			const string Content = "0xFFFF'FFFF'FFFF'FFFFL";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenUnsignedInteger;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 0xFFFFFFFFFFFFFFFF);
		}

		[TestMethod]
		public void TestHexL1()
		{
			const string Content = "0xFFFF_FFFFL";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenSignedInteger;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 0xFFFFFFFFL);
		}

		[TestMethod]
		public void TestHexL2()
		{
			const string Content = "0xFFF'FFFF'FFFF'FFFF";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenSignedInteger;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 0xFFFFFFFFFFFFFFF);
		}

		[TestMethod]
		public void TestHexU1()
		{
			const string Content = "0xFFF_FFFU";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenUnsignedInteger;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 0xFFFFFF);
		}

		[TestMethod]
		public void TestHexU2()
		{
			const string Content = "0xFFF'FFFF'FFFF'FFFFU";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenUnsignedInteger;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 0xFFFFFFFFFFFFFFF);
		}
	}
}
