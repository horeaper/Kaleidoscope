using System.Collections.Immutable;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	/// <summary>
	/// using static System.Math;
	/// </summary>
	public sealed class UsingStaticDirective
	{
		public readonly ImmutableArray<Token> OwnerNamespace; 
		public readonly TokenBlock TypeContent;

		public UsingStaticDirective(Token[] ownerNamespace, TokenBlock typeContent)
		{
			OwnerNamespace = ImmutableArray.Create(ownerNamespace);
			TypeContent = typeContent;
		}

		public override string ToString()
		{
			return $"[UsingStaticDirective] using static {TypeContent.Text};";
		}
	}
}
