using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	public static partial class ReferenceReader
	{
		public static ReferenceToType Read(TokenBlock block, ref int index, TypeParsingRule parsingRule)
		{
			var token = block.GetToken(index, Error.Analysis.IdentifierExpected);
			var identifierToken = token as TokenIdentifier;

			if (token.Type == TokenType.@void) {
				if (!parsingRule.HasFlag(TypeParsingRule.AllowVoid)) {
					throw ParseException.AsToken(token, Error.Analysis.VoidNotAllowed);
				}
				++index;
				return new ReferenceVoid(token);
			}
			else if (token.Type == TokenType.var) {
				if (!parsingRule.HasFlag(TypeParsingRule.AllowVar)) {
					throw ParseException.AsToken(token, Error.Analysis.VarNotAllowed);
				}
				++index;
				return new ReferenceVar(identifierToken);
			}
			else if (identifierToken?.ContextualKeyword == ContextualKeywordType.cpp) {
				var colonToken = block.GetToken(index + 1);
				if (colonToken?.Type == TokenType.DoubleColon) {    //If `cpp` doesn't followed by a `::`, then treat it as normal identifier
					if (!parsingRule.HasFlag(TypeParsingRule.AllowCppType)) {
						throw ParseException.AsRange(block.SourceFile, token.Begin, colonToken.End, Error.Analysis.CppTypeNotAllowed);
					}
					index += 2;
					return ReadCppType(block, ref index, parsingRule.HasFlag(TypeParsingRule.AllowArray));
				}
			}

			if (identifierToken != null || ConstantTable.Alias.Contains(token.Type)) {
				return ReadManagedType(block, ref index, parsingRule.HasFlag(TypeParsingRule.AllowArray));
			}
			else {
				throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
			}
		}

		public static ReferenceToConstant ReadAsConstant(TokenBlock block, ref int index)
		{
			var token = block.GetToken(index, Error.Analysis.UnexpectedToken);
			if (token.Type == TokenType.NumberLiteral ||
				token.Type == TokenType.BooleanLiteral ||
				token.Type == TokenType.CharacterLiteral ||
				token.Type == TokenType.StringLiteral)
			{
				++index;
				return new ReferenceToConstantAsLiteral(token);
			}

			return new ReferenceToConstantAsType(Read(block, ref index, TypeParsingRule.AllowCppType));
		}

		public enum ContentStyle
		{
			None,
			AllowQuestion,
			AllowThis,
			Cpp,
		}

		internal static TokenBlock ReadTypeContent(TokenBlock block, ref int index, ContentStyle style)
		{
			var startIndex = index;

			int arrowCount = 0;
			while (true) {
				var token = block.GetToken(index++, Error.Analysis.IdentifierExpected);

				if (ConstantTable.Alias.Contains(token.Type) ||
					token.Type == TokenType.Identifier ||
					(style == ContentStyle.AllowThis && token.Type == TokenType.@this) ||
					(style == ContentStyle.Cpp && (token.Type == TokenType.NumberLiteral || token.Type == TokenType.StringLiteral) && arrowCount > 0))
				{
					if (!AdvanceIndex(block, ref index, ref arrowCount, style)) {
						break;
					}
				}
				else {
					throw ParseException.AsToken(token, Error.Analysis.IdentifierExpected);
				}
			}

			return block.AsBeginEnd(startIndex, index);
		}

		static bool AdvanceIndex(TokenBlock block, ref int index, ref int arrowCount, ContentStyle style)
		{
			while (true) {
				var token = block.GetToken(index);
				if (token == null) {
					if (arrowCount == 0) {
						return false;
					}
					else {
						throw ParseException.AsToken(block.Last, Error.Analysis.RightArrowExpected);
					}
				}

				if (token.Type == TokenType.Question && style == ContentStyle.AllowQuestion) {
					++index;
					style = ContentStyle.None;
					continue;
				}
				if (token.Type == TokenType.Asterisk && style == ContentStyle.Cpp) {
					++index;
					continue;
				}

				switch (token.Type) {
					case TokenType.Dot:
						++index;
						break;
					case TokenType.DoubleColon:
					case TokenType.PointerArrow:
						if (style != ContentStyle.Cpp) {
							throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
						}
						++index;
						break;
					case TokenType.LeftArrow:
						++index;
						++arrowCount;
						break;
					case TokenType.RightArrow:
						if (arrowCount == 0) {
							return false;
						}

						++index;
						--arrowCount;
						if (arrowCount == 0) {
							token = block.GetToken(index);
							if (token == null) {
								return false;
							}
							else if (token.Type == TokenType.Question && style == ContentStyle.AllowQuestion) {
								++index;
								return false;
							}
							else if (token.Type == TokenType.Dot) {
								++index;
							}
							else {
								return false;
							}
						}
						else {
							continue;
						}
						break;
					case TokenType.ShiftRight:
						if (arrowCount == 0) {
							return false;
						}

						if (arrowCount >= 2) {
							++index;
							arrowCount -= 2;
							if (arrowCount == 0) {
								token = block.GetToken(index);
								if (token != null) {
									return false;
								}
								else if (token.Type == TokenType.Question && style == ContentStyle.AllowQuestion) {
									++index;
									return false;
								}
								else if (token.Type == TokenType.Dot) {
									++index;
								}
								else {
									return false;
								}
							}
							else {
								continue;
							}
						}
						else {
							throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
						}
						break;
					case TokenType.Comma:
						if (arrowCount == 0) {
							return false;
						}
						else {
							++index;
						}
						break;
					default:
						if (arrowCount == 0) {
							return false;
						}
						else {
							throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
						}
				}

				return true;
			}
		}
	}
}
