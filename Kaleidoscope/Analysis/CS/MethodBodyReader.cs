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
		public static void ReadAsMethod(InfoOutput infoOutput, TokenBlock block, ref int index, MethodDeclare.Builder method)
		{
			var token = block.GetToken(index, Error.Analysis.UnexpectedToken);
			if (token.Type == TokenType.Semicolon && method.InstanceKind != MethodInstanceKind.@abstract && method.InstanceKind != MethodInstanceKind.@extern) {
				infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.MethodBodyExpected));
				++index;
			}
			else if (token.Type != TokenType.Semicolon && (method.InstanceKind == MethodInstanceKind.@abstract || method.InstanceKind == MethodInstanceKind.@extern)) {
				var exception = ParseException.AsToken(token, Error.Analysis.MemberCannotHaveBody);
				if (token.Type == TokenType.LeftBrace || token.Type == TokenType.Lambda) {
					infoOutput.OutputError(exception);
				}
				else {
					throw exception;
				}
			}

			switch (token.Type) {
				case TokenType.LeftBrace:
					method.LambdaContentStyle = LambdaStyle.NotLambda;
					method.BodyContent = block.ReadBraceBlock(ref index);
					break;
				case TokenType.Lambda:
					++index;
					token = block.GetToken(index, Error.Analysis.LeftBraceExpected);
					if (token.Type != TokenType.LeftBrace) {
						method.LambdaContentStyle = LambdaStyle.SingleLine;
						method.BodyContent = block.ReadPastSpecificToken(ref index, TokenType.Semicolon, Error.Analysis.SemicolonExpected);
					}
					else {
						method.LambdaContentStyle = LambdaStyle.MultiLine;
						method.BodyContent = block.ReadBraceBlock(ref index);
					}
					break;
				default:
					throw ParseException.AsToken(token, Error.Analysis.MethodBodyExpected);
			}
		}

		public static void ReadAsProperty(InfoOutput infoOutput, TokenBlock block, ref int index, PropertyDeclare.Builder property, PropertyMethodDeclare.Builder method)
		{
			var token = block.GetToken(index, Error.Analysis.LeftBraceExpected);
			if (token.Type == TokenType.Semicolon) {
				method.BodyContent = null;
				++index;
			}
			else if (token.Type != TokenType.Semicolon && property.InstanceKind == PropertyInstanceKind.@abstract) {
				var exception = ParseException.AsToken(token, Error.Analysis.MemberCannotHaveBody);
				if (token.Type == TokenType.LeftBrace) {
					infoOutput.OutputError(exception);
				}
				else {
					throw exception;
				}
			}

			if (token.Type == TokenType.LeftBrace) {
				method.BodyContent = block.ReadBraceBlock(ref index);
			}
			else {
				throw ParseException.AsToken(token, Error.Analysis.AccessorBodyExpected);
			}
		}
	}
}
