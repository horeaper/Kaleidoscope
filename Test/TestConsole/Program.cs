using System;
using Kaleidoscope;
using Kaleidoscope.Analysis;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace TestConsole
{
	class OutputToConsole : OutputToText
	{
		protected override void OnOutputError(string text)
		{
			var colorBefore = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(text);
			Console.ForegroundColor = colorBefore;

		}

		protected override void OnOutputWarning(string text)
		{
			var colorBefore = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(text);
			Console.ForegroundColor = colorBefore;
		}

		protected override void OnOutputMessage(string text)
		{
			var colorBefore = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(text);
			Console.ForegroundColor = colorBefore;
		}
	}

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
			var file = new CodeFile(new OutputToConsole(), new TokenBlock(tokens), LanguageType.CS);
		}
	}
}
