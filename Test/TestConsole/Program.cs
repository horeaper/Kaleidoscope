using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope;
using Kaleidoscope.Analysis;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace TestConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			const string Content = @"
using cpp::IntVec = cpp::std.vector<System.Int64>;
";
			var tokens = Tokenizer.Process(null, new SourceTextFile("", Content), null, false);
			var file = new AnalyzedFile(null, new TokenBlock(tokens), LanguageType.CS);
		}
	}
}
