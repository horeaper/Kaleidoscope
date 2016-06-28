using System.Diagnostics;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class ReferenceToConstantAsLiteral : ReferenceToConstant
	{
		public readonly Token Token;

		public ReferenceToConstantAsLiteral(Token token)
			: base(new TokenBlock(token))
		{
			Debug.Assert(token.Type == TokenType.NumberLiteral ||
						 token.Type == TokenType.BooleanLiteral ||
						 token.Type == TokenType.CharacterLiteral ||
						 token.Type == TokenType.StringLiteral);
			Token = token;
		}
	}
}
