using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	/// <summary>
	/// using cpp::IntVector = cpp::std.vector&lt;int&gt;
	/// using cpp::tr2 = cpp::std.tr2;
	/// </summary>
	public sealed class UsingCppAliasDirective
	{
		public readonly AnalyzedFile Source;
		public readonly Token Name;
		public readonly TokenBlock TypeContent;

		public UsingCppAliasDirective(AnalyzedFile source, Token name, TokenBlock typeContent)
		{
			Source = source;
			Name = name;
			TypeContent = typeContent;
		}

		public override string ToString()
		{
			return $"[UsingCppAliasDirective] using cpp::{Name.Text} = cpp::{TypeContent.Text};";
		}
	}
}
