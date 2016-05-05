using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	/// <summary>
	/// using LookupTable = System.Collections.Generic.Dictionary&lt;int, string&gt;;
	/// Col = System.Collections.Generic;
	/// </summary>
	public sealed class UsingCSAliasDirective
	{
		public readonly AnalyzedFile Source;
		public readonly Token Name;
		public readonly TokenBlock TypeContent;

		public UsingCSAliasDirective(AnalyzedFile source, Token name, TokenBlock typeContent)
		{
			Source = source;
			Name = name;
			TypeContent = typeContent;
		}

		public override string ToString()
		{
			return $"[UsingCSAliasDirective] using {Name.Text} = {TypeContent.Text};";
		}
	}
}
