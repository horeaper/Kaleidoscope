using Kaleidoscope.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestTokenizer
{
	[TestClass]
	public class TestOctalNumber
	{
		[TestMethod]
		public void TestOctalDefault1()
		{
			const string Content = "0o777'777'777";
			var token = Util.Process<TokenSignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 134217727);
		}

		[TestMethod]
		public void TestOctalDefault2()
		{
			const string Content = "0o777777777777";
			var token = Util.Process<TokenSignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 68719476735);
		}

		[TestMethod]
		public void TestOctalLU()
		{
			const string Content = "0o777_777'777'777LU";
			var token = Util.Process<TokenUnsignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 68719476735);
		}

		[TestMethod]
		public void TestOctalU()
		{
			const string Content = "0o7777777777U";
			var token = Util.Process<TokenUnsignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Int);
			Assert.IsTrue(token.Value == 1073741823);
		}

		[TestMethod]
		public void TestOctalL()
		{
			const string Content = "0o7777777777L";
			var token = Util.Process<TokenSignedInteger>(Content);
			Assert.IsTrue(token.Type == IntegerNumberType.Long);
			Assert.IsTrue(token.Value == 1073741823);
		}
	}
}
