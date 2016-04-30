using System;
using Kaleidoscope.Primitive;
using Kaleidoscope.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestTokenizer
{
	[TestClass]
	public class UnitTestHexNumber
	{
		[TestMethod]
		public void TestDefaultHex()
		{
			const string Content = "0xFF_FF_FF_FF";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenUnsignedNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 0xFFFFFFFF);
		}

		[TestMethod]
		public void TestDefaultHex2()
		{
			const string Content = "0x0F'FF'FF'FF";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenSignedNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 0x0FFFFFFF);
		}

		[TestMethod]
		public void TestULHex()
		{
			const string Content = "0xFFFFFFFFUL";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenUnsignedNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 0xFFFFFFFFUL);
		}

		[TestMethod]
		public void TestULHex2()
		{
			const string Content = "0xFFFF'FFFF'FFFF'FFFFL";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenUnsignedNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 0xFFFFFFFFFFFFFFFF);
		}

		[TestMethod]
		public void TestLHex()
		{
			const string Content = "0xFFFF_FFFFL";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenSignedNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 0xFFFFFFFFL);
		}

		[TestMethod]
		public void TestLHex2()
		{
			const string Content = "0xFFF'FFFF'FFFF'FFFF";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenSignedNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 0xFFFFFFFFFFFFFFF);
		}

		[TestMethod]
		public void TestUHex()
		{
			const string Content = "0xFFF_FFFU";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenUnsignedNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 0xFFFFFF);
		}

		[TestMethod]
		public void TestUHex2()
		{
			const string Content = "0xFFF'FFFF'FFFF'FFFFU";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenUnsignedNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 0xFFFFFFFFFFFFFFF);
		}
	}
}
