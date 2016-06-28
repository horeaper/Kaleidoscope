using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	/// <summary>
	/// using cpp::IntVector = cpp::std.vector&lt;int&gt;
	/// using cpp::tr2 = cpp::std.tr2;
	/// </summary>
	public sealed class UsingCppAliasDirective
	{
		public readonly Token Name;
		public readonly ReferenceToCppType Type;

		public UsingCppAliasDirective(Token name, ReferenceToCppType type)
		{
			Name = name;
			Type = type;
		}

		public override string ToString()
		{
			return $"[UsingCppAliasDirective] using cpp::{Name.Text} = cpp::{Type.Content.Text};";
		}
	}
}
