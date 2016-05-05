using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	/// <summary>
	/// using static System.Math;
	/// </summary>
	public sealed class UsingStaticDirective
	{
		public readonly AnalyzedFile Source;
		public readonly TokenBlock TypeContent;

		public UsingStaticDirective(AnalyzedFile source, TokenBlock typeContent)
		{
			Source = source;
			TypeContent = typeContent;
		}

		public override string ToString()
		{
			return $"[UsingStaticDirective] using static {TypeContent.Text};";
		}
	}
}
