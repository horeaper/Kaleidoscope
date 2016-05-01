using System;
using Kaleidoscope.Primitive;
using Kaleidoscope.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestTokenizer
{
	[TestClass]
	public class TestRealNumber
	{
		[TestMethod]
		public void TestReal1()
		{
			const string Content = ".1234567";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenFloatNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == FloatNumberType.Double);
			Assert.IsTrue(token.Value == 0.1234567);
		}

		[TestMethod]
		public void TestReal2()
		{
			const string Content = ".1234567f";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenFloatNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == FloatNumberType.Float);
			Assert.IsTrue(token.Value == 0.1234567f);
		}

		[TestMethod]
		public void TestReal3()
		{
			const string Content = "3.1415926";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenFloatNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == FloatNumberType.Double);
			Assert.IsTrue(token.Value == 3.1415926);
		}

		[TestMethod]
		public void TestReal4()
		{
			const string Content = "3.1415926F";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenFloatNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == FloatNumberType.Float);
			Assert.IsTrue(token.Value == 3.1415926F);
		}

		[TestMethod]
		public void TestReal5()
		{
			const string Content = "3.1415926d";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenFloatNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == FloatNumberType.Double);
			Assert.IsTrue(token.Value == 3.1415926d);
		}

		[TestMethod]
		public void TestReal6()
		{
			const string Content = "-3.1415926M";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenDecimalNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Value == -3.1415926M);
		}

		[TestMethod]
		public void TestReal7()
		{
			const string Content = "1.732e11";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenFloatNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == FloatNumberType.Double);
			Assert.IsTrue(token.Value == 1.732e11);
		}

		[TestMethod]
		public void TestReal8()
		{
			const string Content = "-17.32'34'36e20f";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenFloatNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Type == FloatNumberType.Float);
			Assert.IsTrue(token.Value == -17.323436e20f);
		}

		[TestMethod]
		public void TestReal9()
		{
			const string Content = "148.367e-22m";
			var tokens = Tokenizer.Process(new SourceTextFile("", Content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as TokenDecimalNumber;
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Value == 148.367e-22m);
		}
	}
}
