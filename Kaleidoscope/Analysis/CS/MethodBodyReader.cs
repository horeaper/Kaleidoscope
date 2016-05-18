using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	static class MethodBodyReader
	{
		public static void Read(TokenBlock block, ref int index, MethodDeclare.Builder builder)
		{
			var token = block.GetToken(index);
			switch (token.Type) {
				case TokenType.LeftBrace:
					builder.LambdaContentStyle = LambdaStyle.NotLambda;
					builder.BodyContent = block.ReadBraceBlock(ref index);
					break;
				case TokenType.Lambda:
					++index;
					token = block.GetToken(index, Error.Analysis.LeftBraceExpected);
					if (token.Type != TokenType.LeftBrace) {
						builder.LambdaContentStyle = LambdaStyle.SingleLine;
						builder.BodyContent = block.ReadPastSpecificToken(ref index, TokenType.Semicolon, Error.Analysis.SemicolonExpected);
					}
					else {
						builder.LambdaContentStyle = LambdaStyle.MultiLine;
						builder.BodyContent = block.ReadBraceBlock(ref index);
					}
					break;
				default:
					throw ParseException.AsToken(token, Error.Analysis.MethodBodyExpected);
			}
		}
	}
}
