using System;

namespace Kaleidoscope
{
	class OutputByConsole : DefaultInfoOutput
	{
		public override void OnOutputError(string text)
		{
			lock (this) {
				var colorBefore = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(text);
				Console.ForegroundColor = colorBefore;

			}
		}

		public override void OnOutputWarning(string text)
		{
			lock (this) {
				var colorBefore = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(text);
				Console.ForegroundColor = colorBefore;
			}
		}

		public override void OnOutputMessage(string text)
		{
			lock (this) {
				var colorBefore = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(text);
				Console.ForegroundColor = colorBefore;
			}
		}

		public override void OnOutputVerbose(string text)
		{
			lock (this) {
				Console.WriteLine(text);
			}
		}
	}
}
