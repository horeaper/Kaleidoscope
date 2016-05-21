using System;
using Kaleidoscope;
using Kaleidoscope.Analysis;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;
using System.Collections.Immutable;

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
		}
	}
}
