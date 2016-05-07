using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class TypeReferenceReader
	{
		static ReferenceToCppType ReadCppType(TokenBlock block, ref int index, bool isAllowArray)
		{
			var builder = new ReferenceToCppType.Builder();
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
			builder.Content = ReadTypeContent(block, ref index, ContentStyle.AllowAsterisk);

			//Pointer
			for (int cnt = builder.Content.Count - 1; cnt >= 0; --cnt) {
				if (builder.Content[cnt].Type == TokenType.Asterisk) {
					++builder.PointerRank;
				}
				else {
					break;
				}
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

							++index;
							builder.ArrayItemNumber.Add(InstanceReferenceReader.Read(block, ref index, true));
						}
						else if (token.Type == TokenType.RightBracket) {
							if (!isInsideBracket) {
								throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
							}
							isInsideBracket = false;
						}
					}
				}
			}

			return new ReferenceToCppType(builder);
		}
	}
}
