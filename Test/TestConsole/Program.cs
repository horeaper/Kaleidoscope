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
			const string Content = 
@"
[Attribute(""SomeData"", First = ""First"", Second = 17, Third = new int[0])]
[Another]
";
			var tokens = Tokenizer.Process(null, new SourceTextFile("", Content), null, false, false);
			var file = new AnalyzedFile(new TokenBlock(tokens), LanguageType.CS);
		}
	}
}
