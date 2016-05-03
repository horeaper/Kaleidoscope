using System;
using Kaleidoscope.Primitive;
using Kaleidoscope.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestTokenizer
{
	[TestClass]
	public class TestIntegerNumber
	{
		[TestMethod]
		public void TestIntegerDefault1()
		{
			const string Content = "1234567";
			var token = Util.Process<TokenSignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 1234567);
		}

		[TestMethod]
		public void TestIntegerDefault2()
		{
			const string Content = "-1234567";
			var token = Util.Process<TokenSignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == -1234567);
		}

		[TestMethod]
		public void TestIntegerDefault3()
		{
			const string Content = "4294967295";
			var token = Util.Process<TokenUnsignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 4294967295);
		}

		[TestMethod]
		public void TestIntegerDefault4()
		{
			const string Content = "-4294967295";
			var token = Util.Process<TokenSignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == -4294967295);
		}

		[TestMethod]
		public void TestIntegerDefault5()
		{
			const string Content = "1099511627775";
			var token = Util.Process<TokenSignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 1099511627775);
		}

		[TestMethod]
		public void TestIntegerUL()
		{
			const string Content = "1'0000'0000UL";
			var token = Util.Process<TokenUnsignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 100000000UL);
		}

		[TestMethod]
		public void TestIntegerU()
		{
			const string Content = "854U";
			var token = Util.Process<TokenUnsignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 854U);
		}

		[TestMethod]
		public void TestIntegerL1()
		{
			const string Content = "-1_0000_0000L";
			var token = Util.Process<TokenSignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == -100000000L);
		}

		[TestMethod]
		public void TestIntegerL2()
		{
			const string Content = "1L";
			var token = Util.Process<TokenSignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 1L);
		}

		[TestMethod]
		public void TestIntegerM1()
		{
			const string Content = "-1_0000_0000M";
			var token = Util.Process<TokenDecimalNumber>(Content);
			Assert.IsTrue(token.Value == -100000000M);
		}

		[TestMethod]
		public void TestIntegerM2()
		{
			const string Content = "123456789m";
			var token = Util.Process<TokenDecimalNumber>(Content);
			Assert.IsTrue(token.Value == 123456789m);
		}
	}
}
