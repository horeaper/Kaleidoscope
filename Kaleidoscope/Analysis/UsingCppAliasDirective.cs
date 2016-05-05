using System.Collections.Immutable;
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
		public readonly ImmutableArray<Token> OwnerNamespace;
		public readonly Token Name;
		public readonly ReferenceToCppType Type;

		public UsingCppAliasDirective(Token[] ownerNamespace, Token name, ReferenceToCppType type)
		{
			OwnerNamespace = ImmutableArray.Create(ownerNamespace);
			Name = name;
			Type = type;
		}

		public override string ToString()
		{
			return $"[UsingCppAliasDirective] using cpp::{Name.Text} = cpp::{Type.Content.Text};";
		}
	}
}
