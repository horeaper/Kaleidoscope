using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.SyntaxObject
{
	public sealed class CppTemplateItemLiteral : CppTemplateItem
	{
		public readonly Token Token;
		public override string DisplayName => Token.Text;

		public CppTemplateItemLiteral(Token token)
		{
			Debug.Assert(token.Type == TokenType.NumberLiteral ||
						 token.Type == TokenType.StringLiteral ||
						 token.Type == TokenType.Character ||
						 token.Type == TokenType.BooleanLiteral);
			Token = token;
		}
	}
}
