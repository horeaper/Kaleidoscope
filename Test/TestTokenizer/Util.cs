using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.Primitive;
using Kaleidoscope.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestTokenizer
{
	static class Util
	{
		public static T Process<T>(string content) where T : class
		{
			var tokens = Tokenizer.Process(new SourceTextFile("", content), null);
			Assert.IsTrue(tokens.Length == 1);
			var token = tokens[0] as T;
			Assert.IsNotNull(token);
			return token;
		}
	}
}
