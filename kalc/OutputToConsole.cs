using System;

namespace Kaleidoscope
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
	}
}
