using System;
using Kaleidoscope.Primitive;
using Kaleidoscope.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestTokenizer
{
	[TestClass]
	public class TestCharString
	{
		[TestMethod]
		public void TestChar1()
		{
			const string Content = "'K'";
			var token = Util.Process<TokenCharacter>(Content);
			Assert.IsTrue(token.Value == 'K');
		}

		[TestMethod]
		public void TestChar2()
		{
			const string Content = "'\\n'";
			var token = Util.Process<TokenCharacter>(Content);
			Assert.IsTrue(token.Value == '\n');
		}

		[TestMethod]
		public void TestChar3()
		{
			const string Content = "'\\x0A'";
			var token = Util.Process<TokenCharacter>(Content);
			Assert.IsTrue(token.Value == 0x0A);
		}

		[TestMethod]
		public void TestChar4()
		{
			const string Content = "'\\u597D'";
			var token = Util.Process<TokenCharacter>(Content);
			Assert.IsTrue(token.Value == '好');
		}

		[TestMethod]
		public void TestString1()
		{
			const string Content = "\"Some\\tString\"";
			var token = Util.Process<TokenString>(Content);
			Assert.IsTrue(token.ConvertedText == "Some\tString");
		}

		[TestMethod]
		public void TestString2()
		{
			const string Content = "\"\"";
			var token = Util.Process<TokenString>(Content);
			Assert.IsTrue(token.ConvertedText == "");
		}

		[TestMethod]
		public void TestString3()
		{
			const string Content = "\"哈哈哈哈哈哈\\u54C8\"";
			var token = Util.Process<TokenString>(Content);
			Assert.IsTrue(token.ConvertedText == "哈哈哈哈哈哈哈");
		}

		[TestMethod]
		public void TestVerbatimString1()
		{
			const string Content = "@\"123456\\t\"";
			var token = Util.Process<TokenString>(Content);
			Assert.IsTrue(token.ConvertedText == "123456\\t");
		}

		[TestMethod]
		public void TestVerbatimString2()
		{
			const string Content = "@\"First\nSecond\"";
			var token = Util.Process<TokenString>(Content);
			Assert.IsTrue(token.ConvertedText == "First\nSecond");
		}

		[TestMethod]
		public void TestVerbatimString3()
		{
			const string Content = "@\"Left\"\"Right\"";
			var token = Util.Process<TokenString>(Content);
			Assert.IsTrue(token.ConvertedText == @"Left""Right");
		}
	}
}
