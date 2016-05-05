using Kaleidoscope;
using Kaleidoscope.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestTokenizer
{
	static class Util
	{
		public static T Process<T>(string content) where T : class
		{
			var tokens = Tokenizer.Process(null, new SourceTextFile("", content), null, true);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as T;
			Assert.IsNotNull(token);
			return token;
		}
	}
}
