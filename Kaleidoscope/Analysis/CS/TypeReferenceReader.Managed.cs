using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class TypeReferenceReader
	{
		static ReferenceToManagedType ReadManagedType(TokenBlock block, ref int index, bool isAllowArray)
		{
			var builder = new ReferenceToManagedType.Builder();
			var token = block.GetToken(index);

			//global::
			if ((token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.global) {
				var colonToken = block.GetToken(index + 1);
				if (colonToken?.Type == TokenType.DoubleColon) {
					builder.IsGlobalNamespace = true;
					index += 2;
				}
			}

			//Content
			builder.Content = ReadTypeContent(block, ref index, ContentStyle.AllowQuestion);

			//Nullable
			if (builder.Content.Last.Type == TokenType.Question) {
				builder.IsNullable = true;
			}

			//Array
			if (isAllowArray) {
				token = block.GetToken(index);
				if (token?.Type == TokenType.LeftBracket) {
					bool isInsideBracket = false;
					while (true) {
						if (token.Type == TokenType.LeftBracket) {
							if (isInsideBracket) {
								throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
							}
							isInsideBracket = true;
							builder.ArrayDimensions.Add(1);
						}
						else if (token.Type == TokenType.RightBracket) {
							if (!isInsideBracket) {
								throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
							}
							isInsideBracket = false;
						}
						else if (token.Type == TokenType.Comma) {
							if (!isInsideBracket) {
								throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
							}
							++builder.ArrayDimensions[builder.ArrayDimensions.Count - 1];
						}
						else {
							if (isInsideBracket) {
								throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
							}
							break;
						}

						++index;
						var lastToken = token;
						token = block.GetToken(index);
						if (token == null) {
							if (!isInsideBracket) {
								break;
							}
							else {
								throw ParseException.AsToken(lastToken, Error.Analysis.RightBracketExpected);
							}
						}
					}
				}
			}

			return new ReferenceToManagedType(builder);
		}
	}
}
