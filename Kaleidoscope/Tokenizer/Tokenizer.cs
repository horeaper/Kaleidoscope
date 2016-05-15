using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Kaleidoscope.Preprocessor;

namespace Kaleidoscope.Tokenizer
{
	public static class Tokenizer
	{
		public static ImmutableArray<Token> Process(InfoOutput infoOutput, SourceTextFile source, IEnumerable<string> definedSymbols, bool isIncludeTrivia, bool isIncludeComment)
		{
			var builder = ImmutableArray.CreateBuilder<Token>();

			var currentSymbols = new SortedSet<string>(definedSymbols ?? new string[] { });
			int index = 0;
			while (index < source.Length) {
				var token = GetTokenWorker(infoOutput, source, index);
				if (token == null) {
					break;
				}

				var preprocessorToken = token as TokenPreprocessor;
				if (preprocessorToken != null) {
					index = token.End;
					ProcessPreprocessor(infoOutput, source, currentSymbols, preprocessorToken, ref index);
				}
				else {
					if (token is TokenTrivia && !isIncludeTrivia) {
						index = token.End;
					}
					else if (token is TokenComment && !isIncludeComment) {
						index = token.End;
					}
					else {
						builder.Add(token);
						index = token.End;
					}
				}
			}

			builder.Capacity = builder.Count;
			return builder.MoveToImmutable();
		}

		static Token GetTokenWorker(InfoOutput infoOutput, SourceTextFile source, int index)
		{
			//Ignores
			while (index < source.Length && source[index] == '\r') {
				++index;    //carrage return
			}
			if (index >= source.Length) {
				return null;
			}
			if (source[index] == '\0') {
				return null;
			}

			//Trivia
			if (source[index] == '\n') {
				return new TokenTrivia(source, index, index + 1, TriviaType.NewLine);
			}
			if (source[index] == ' ') {
				int startIndex = index;
				while (index < source.Length && source[index] == ' ') {
					++index;
				}
				return new TokenTrivia(source, startIndex, index, TriviaType.Space);
			}
			if (source[index] == '\t') {
				int startIndex = index;
				while (index < source.Length && source[index] == '\t') {
					++index;
				}
				return new TokenTrivia(source, startIndex, index, TriviaType.Tab);
			}

			try {
				#region Hex/Binary/Octal Number

				if (source[index] == '0') {
					if (index + 1 >= source.Length) {
						return new TokenSignedInteger(source, index, index + 1, 0, IntegerNumberType.Int);
					}

					Func<NumberTestDelegate, NumberConvertDelegate, Token> fnParseSpecialNumber = (fnNumberTest, fnConvertText) => {
						int startIndex = index;
						index += 2;
						if (!fnNumberTest(source[index])) {
							throw ParseException.AsIndex(source, index, Error.Tokenizer.UnknownToken);
						}

						var numberText = ReadNumberText(source, ref index, fnNumberTest);
						var trailing = ReadIntegerLiteralTrailing(source, ref index);
						switch (trailing) {
							case IntegerTrailingType.UL:
								{
									ulong value;
									if (!fnConvertText(numberText, out value)) {
										infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant));
									}
									return new TokenUnsignedInteger(source, startIndex, index, value, IntegerNumberType.Long);
								}
							case IntegerTrailingType.U:
								{
									ulong value;
									if (!fnConvertText(numberText, out value)) {
										infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant));
									}

									if (value > uint.MaxValue) {
										return new TokenUnsignedInteger(source, startIndex, index, value, IntegerNumberType.Long);
									}
									else {
										return new TokenUnsignedInteger(source, startIndex, index, (uint)value, IntegerNumberType.Int);
									}
								}
							case IntegerTrailingType.L:
								{
									ulong value;
									if (!fnConvertText(numberText, out value)) {
										infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant));
									}

									if (value > long.MaxValue) {
										return new TokenUnsignedInteger(source, startIndex, index, value, IntegerNumberType.Long);
									}
									else {
										return new TokenSignedInteger(source, startIndex, index, (long)value, IntegerNumberType.Long);
									}
								}
							default:
								{
									ulong value;
									if (!fnConvertText(numberText, out value)) {
										infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant));
									}

									if (value > long.MaxValue) {
										return new TokenUnsignedInteger(source, startIndex, index, value, IntegerNumberType.Long);
									}
									else if (value > uint.MaxValue) {
										return new TokenSignedInteger(source, startIndex, index, (long)value, IntegerNumberType.Long);
									}
									else if (value > int.MaxValue) {
										return new TokenUnsignedInteger(source, startIndex, index, (uint)value, IntegerNumberType.Int);
									}
									else {
										return new TokenSignedInteger(source, startIndex, index, (int)value, IntegerNumberType.Int);
									}
								}
						}
					};

					//Test for hex number
					if (source[index + 1] == 'x' || source[index + 1] == 'X') {
						return fnParseSpecialNumber(IsHexNumber, ConvertHexNumberString);
					}

					//Test for binary number
					if (source[index + 1] == 'b' || source[index + 1] == 'b') {
						return fnParseSpecialNumber(IsBinaryNumber, ConvertBinaryNumberString);
					}

					//Test for octal number
					if (source[index + 1] == 'o' || source[index + 1] == 'O') {
						return fnParseSpecialNumber(IsOctalNumber, ConvertOctalNumberString);
					}
				}

				#endregion

				#region Standard number

				if (source[index] == '-' || source[index] == '.' || IsNumber(source[index])) {
					int startIndex = index;

					//Create leading text (minus sign and/or decimal point)
					bool isMinusFirst = false;
					if (source[index] == '-') {
						isMinusFirst = true;
						++index;
						if (!IsNumber(source[index])) {
							return new TokenSymbol(source, startIndex, startIndex + 1, SymbolType.Minus);
						}
					}
					bool isDotFirst = false;
					if (source[index] == '.') {
						isDotFirst = true;
						++index;
						if (!IsNumber(source[index])) {
							return new TokenSymbol(source, startIndex, startIndex + 1, SymbolType.Dot);
						}
					}
					var leadingText = "";
					if (isMinusFirst) {
						leadingText += "-";
					}
					if (isDotFirst) {
						leadingText += "0.";
					}

					Func<string, Token> fnParseIntegerNumber = text => {
						if (!isMinusFirst) {
							ulong value;
							if (!ulong.TryParse(text, NumberStyles.None, CultureInfo.CurrentCulture, out value)) {
								infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant));
							}

							if (value > long.MaxValue) {
								return new TokenUnsignedInteger(source, startIndex, index, value, IntegerNumberType.Long);
							}
							else if (value > uint.MaxValue) {
								return new TokenSignedInteger(source, startIndex, index, (long)value, IntegerNumberType.Long);
							}
							else if (value > int.MaxValue) {
								return new TokenUnsignedInteger(source, startIndex, index, (uint)value, IntegerNumberType.Int);
							}
							else {
								return new TokenSignedInteger(source, startIndex, index, (int)value, IntegerNumberType.Int);
							}
						}
						else {
							long value;
							if (!long.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.CurrentCulture, out value)) {
								infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant));
							}

							if (value < int.MinValue) {
								return new TokenSignedInteger(source, startIndex, index, value, IntegerNumberType.Long);
							}
							else {
								return new TokenSignedInteger(source, startIndex, index, value, IntegerNumberType.Int);
							}
						}
					};

					Func<string, Token> fnParseRealNumber = text => {
						double value;
						if (!double.TryParse(text, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value)) {
							infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant));
						}
						return new TokenFloatNumber(source, startIndex, index, value, FloatNumberType.Double);
					};

					Func<string, Token> fnParseNumber = text =>
						!isDotFirst ? fnParseIntegerNumber(text) : fnParseRealNumber(text);

					var partNumber = ReadNumberText(source, ref index, IsNumber);
					string numberText = leadingText + partNumber;
					if (index >= source.Length) {
						return fnParseNumber(numberText);
					}

					var integerTrailing = ReadIntegerLiteralTrailing(source, ref index);
					if (integerTrailing != IntegerTrailingType.None) {
						if (isDotFirst) {
							infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant));
							return new TokenFloatNumber(source, startIndex, index, -1, FloatNumberType.Double);
						}
						if (isMinusFirst && (integerTrailing == IntegerTrailingType.UL || integerTrailing == IntegerTrailingType.U)) {
							infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant));
							return new TokenSignedInteger(source, startIndex, index, -1, IntegerNumberType.Int);
						}

						switch (integerTrailing) {
							case IntegerTrailingType.UL:
								{
									ulong value;
									if (!ulong.TryParse(numberText, NumberStyles.None, CultureInfo.CurrentCulture, out value)) {
										infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant));
									}
									return new TokenUnsignedInteger(source, startIndex, index, value, IntegerNumberType.Long);
								}
							case IntegerTrailingType.U:
								{
									ulong value;
									if (!ulong.TryParse(numberText, NumberStyles.None, CultureInfo.CurrentCulture, out value)) {
										infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant));
									}

									if (value > uint.MaxValue) {
										return new TokenUnsignedInteger(source, startIndex, index, value, IntegerNumberType.Long);
									}
									else {
										return new TokenUnsignedInteger(source, startIndex, index, (uint)value, IntegerNumberType.Int);
									}
								}
							case IntegerTrailingType.L:
								{
									if (!isMinusFirst) {
										ulong value;
										if (!ulong.TryParse(numberText, NumberStyles.None, CultureInfo.CurrentCulture, out value)) {
											infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant));
										}

										if (value > long.MaxValue) {
											return new TokenUnsignedInteger(source, startIndex, index, value, IntegerNumberType.Long);
										}
										else {
											return new TokenSignedInteger(source, startIndex, index, (long)value, IntegerNumberType.Long);
										}
									}
									else {
										long value;
										if (!long.TryParse(numberText, NumberStyles.AllowLeadingSign, CultureInfo.CurrentCulture, out value)) {
											infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant));
										}
										return new TokenSignedInteger(source, startIndex, index, value, IntegerNumberType.Long);
									}
								}
						}
					}

					Func<Func<string, Token>, Token> fnCreateNumberToken = fnDefaultNumberParser => {
						switch (source[index]) {
							case 'F':
							case 'f':
								{
									++index;
									float value;
									if (!float.TryParse(numberText, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value)) {
										infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant));
									}
									return new TokenFloatNumber(source, startIndex, index, value, FloatNumberType.Float);
								}
							case 'D':
							case 'd':
								{
									++index;
									double value;
									if (!double.TryParse(numberText, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value)) {
										infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant));
									}
									return new TokenFloatNumber(source, startIndex, index, value, FloatNumberType.Double);
								}
							case 'M':
							case 'm':
								{
									++index;
									decimal value;
									if (!decimal.TryParse(numberText, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value)) {
										infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidDecimalConstant));
									}
									return new TokenDecimalNumber(source, startIndex, index, value);
								}
							case 'E':
							case 'e':
								{
									++index;
									bool isExpNegative = false;
									if (source[index] == '-') {
										isExpNegative = true;
										++index;
									}
									var partExponent = ReadNumberText(source, ref index, IsNumber);
									if (isExpNegative) {
										partExponent = '-' + partExponent;
									}
									numberText += "e" + partExponent;

									if (index >= source.Length) {
										double value;
										if (!double.TryParse(numberText, NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value)) {
											infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant));
										}
										return new TokenFloatNumber(source, startIndex, index, value, FloatNumberType.Double);
									}

									switch (source[index]) {
										case 'F':
										case 'f':
											{
												++index;
												float value;
												if (!float.TryParse(numberText, NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value)) {
													infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant));
												}
												return new TokenFloatNumber(source, startIndex, index, value, FloatNumberType.Float);
											}
										case 'D':
										case 'd':
											{
												++index;
												double value;
												if (!double.TryParse(numberText, NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value)) {
													infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant));
												}
												return new TokenFloatNumber(source, startIndex, index, value, FloatNumberType.Double);
											}
										case 'M':
										case 'm':
											{
												++index;
												decimal value;
												if (!decimal.TryParse(numberText, NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value)) {
													infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidDecimalConstant));
												}
												return new TokenDecimalNumber(source, startIndex, index, value);
											}
										default:
											{
												double value;
												if (!double.TryParse(numberText, NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value)) {
													infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant));
												}
												return new TokenFloatNumber(source, startIndex, index, value, FloatNumberType.Double);
											}
									}
								}
							default:
								return fnDefaultNumberParser(numberText);
						}
					};

					if (source[index] == '.') {
						if (isDotFirst || !IsNumber(source[index + 1])) {
							return fnParseNumber(numberText);
						}
						++index;

						var partDecimal = ReadNumberText(source, ref index, IsNumber);
						numberText += '.' + partDecimal;
						if (index >= source.Length) {
							return fnParseRealNumber(numberText);
						}

						return fnCreateNumberToken(fnParseRealNumber);
					}
					else {
						return fnCreateNumberToken(fnParseNumber);
					}
				}

				#endregion

				#region Character

				if (source[index] == '\'') {
					int startIndex = index;
					++index;

					char result;
					switch (source[index]) {
						case '\\':
							++index;
							result = ReadCharacterLiterialEscapeSequence(infoOutput, source, startIndex, ref index);
							break;
						case '\r':
						case '\n':
							throw ParseException.AsIndex(source, index, Error.Tokenizer.NewLineNotAllowedOnChar);
						case '\'':
							infoOutput.OutputError(ParseException.AsRange(source, startIndex, index, Error.Tokenizer.EmptyCharLiteral));
							result = (char)0;
							break;
						default:
							result = source[index];
							++index;
							break;
					}
					if (source[index] != '\'') {
						throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.BadCharConstant);
					}

					return new TokenCharacter(source, startIndex, index + 1, result);
				}

				#endregion

				#region String

				if (source[index] == '\"') {
					int startIndex = index;
					++index;

					var convertedContent = new StringBuilder();
					while (true) {
						switch (source[index]) {
							case '\\':
								++index;
								convertedContent.Append(ReadCharacterLiterialEscapeSequence(infoOutput, source, startIndex, ref index));
								break;
							case '\r':
							case '\n':
								infoOutput.OutputError(ParseException.AsIndex(source, index, Error.Tokenizer.NewLineNotAllowedOnString));
								++index;
								break;
							case '\"':
								return new TokenString(source, startIndex, index + 1, convertedContent.ToString());
							default:
								convertedContent.Append(source[index]);
								++index;
								break;
						}
					}
				}

				#endregion

				#region Verbatim string

				if (source[index] == '@') {
					int startIndex = index;
					if (source[index + 1] == '\"') {
						++index;

						var convertedContent = new StringBuilder();
						while (true) {
							++index;

							if (source[index] == '\"') {
								++index;
								if (index >= source.Length || source[index] != '\"') {
									return new TokenString(source, startIndex, index, convertedContent.ToString());
								}
								else {
									convertedContent.Append(source[index]);
								}
							}
							else {
								convertedContent.Append(source[index]);
							}
						}
					}
				}

				#endregion

				#region Comment

				if (source[index] == '/' && index + 1 < source.Length) {
					if (source[index + 1] == '/') {
						int startIndex = index;
						index += 2;
						while (index < source.Length && source[index] != '\r' && source[index] != '\n') {
							++index;
						}

						return new TokenComment(source, startIndex, index, false);
					}
					else if (source[index + 1] == '*') {
						int startIndex = index;
						int endIndex = source.FileContent.IndexOf("*/", index + 2, StringComparison.CurrentCulture);
						if (endIndex == -1) {
							throw ParseException.AsEOF(source, Error.Tokenizer.MultiLineCommentNotClosed);
						}

						return new TokenComment(source, startIndex, endIndex + 2, true);
					}
				}

				#endregion

				#region Preprocessor

				if (source[index] == '#') {
					int startIndex = index;
					++index;

					while (source[index] == ' ' || source[index] == '\t') {
						++index;
					}
					int identifierStart = index;
					while (source[index] >= 'a' && source[index] <= 'z') {
						++index;
						if (index >= source.Length) {
							break;
						}
					}
					string preprocessorTypeText = source.Substring(identifierStart, index);

					PreprocessorType type;
					if (!Enum.TryParse(preprocessorTypeText, false, out type)) {
						throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidPreprocessor);
					}

					var contentTokens = new List<Token>();
					var token = GetTokenWorker(infoOutput, source, index);
					while (token != null) {
						var triviaToken = token as TokenTrivia;
						if (triviaToken != null) {
							if (triviaToken.Type == TriviaType.NewLine) {
								index = token.End;
								break;
							}
						}
						else {
							contentTokens.Add(token);
						}

						index = token.End;
						token = GetTokenWorker(infoOutput, source, index);
					}
					return new TokenPreprocessor(source, startIndex, index, type, contentTokens.ToArray());
				}

				#endregion

				#region Symbol

				SymbolType symbol;
				var symbolContent = new StringBuilder();
				symbolContent.Append(source[index]);
				if (index + 1 < source.Length) {
					symbolContent.Append(source[index + 1]);
					if (ConstantTable.SymbolMap.TryGetValue(symbolContent.ToString(), out symbol)) {
						return new TokenSymbol(source, index, index + symbolContent.Length, symbol);
					}
					symbolContent.Remove(symbolContent.Length - 1, 1);
				}
				if (ConstantTable.SymbolMap.TryGetValue(symbolContent.ToString(), out symbol)) {
					return new TokenSymbol(source, index, index + symbolContent.Length, symbol);
				}

				#endregion

				#region Keyword as Identifier

				if (source[index] == '@') {
					if (IsValidIdentifierCharacter(source[index + 1])) {
						++index;
						int startIndex = index;
						while (index < source.Length && IsValidIdentifierCharacter(source[index])) {
							++index;
						}
						string text = source.Substring(startIndex, index);
						KeywordType keyword;
						bool isKeyword = Enum.TryParse(text, out keyword);
						return new TokenIdentifier(source, startIndex, index, isKeyword);
					}
					else {
						throw ParseException.AsIndex(source, index, Error.Tokenizer.UnknownToken);
					}
				}

				#endregion

				#region Identifier

				if (IsValidIdentifierCharacter(source[index])) {
					int startIndex = index;
					while (index < source.Length && IsValidIdentifierCharacter(source[index])) {
						++index;
					}
					string text = source.Substring(startIndex, index);

					KeywordType keyword;
					if (Enum.TryParse(text, out keyword)) {
						if (keyword == KeywordType.@true || keyword == KeywordType.@false) {
							return new TokenBooleanLiteral(source, startIndex, index, keyword);
						}
						else {
							return new TokenKeyword(source, startIndex, index, keyword);
						}
					}
					else {
						return new TokenIdentifier(source, startIndex, index, false);
					}
				}

				#endregion
			}
			catch (IndexOutOfRangeException) {
				throw ParseException.AsEOF(source, Error.Tokenizer.UnexpectedEOF);
			}

			throw ParseException.AsIndex(source, index, Error.Tokenizer.UnknownToken);
		}

#region Preprocessor

		static void ProcessPreprocessor(InfoOutput infoOutput, SourceTextFile source, SortedSet<string> currentSymbols, TokenPreprocessor preprocessorToken, ref int index)
		{
			switch (preprocessorToken.Type) {
				case PreprocessorType.@define:
					{
						if (preprocessorToken.ContentTokens.Length != 1) {
							infoOutput.OutputError(ParseException.AsToken(preprocessorToken, Error.Tokenizer.InvalidPreprocessor));
							break;
						}
						var token = preprocessorToken.ContentTokens[0] as TokenIdentifier;
						if (token == null) {
							infoOutput.OutputError(ParseException.AsToken(preprocessorToken, Error.Tokenizer.InvalidPreprocessor));
							break;
						}
						currentSymbols.Add(token.Text);
					}
					break;
				case PreprocessorType.@undef:
					{
						if (preprocessorToken.ContentTokens.Length != 1) {
							infoOutput.OutputError(ParseException.AsToken(preprocessorToken, Error.Tokenizer.InvalidPreprocessor));
							break;
						}
						var token = preprocessorToken.ContentTokens[0] as TokenIdentifier;
						if (token == null) {
							infoOutput.OutputError(ParseException.AsToken(preprocessorToken, Error.Tokenizer.InvalidPreprocessor));
							break;
						}
						currentSymbols.Remove(token.Text);
					}
					break;
				case PreprocessorType.@if:
					{
						int tokenIndex = 0;
						var exp = ReadPreprocessorExpression(preprocessorToken, currentSymbols, ref tokenIndex);
						if (!exp.Evaluate) {
							//Find else/elif/endif
							while (true) {
								var token = GetTokenWorker(infoOutput, source, index);
								if (token == null) {
									return;
								}
								index = token.End;

								preprocessorToken = token as TokenPreprocessor;
								if (preprocessorToken != null) {
									switch (preprocessorToken.Type) {
										case PreprocessorType.@else:
										case PreprocessorType.@endif:
											return;
										case PreprocessorType.@elif:
											tokenIndex = 0;
											exp = ReadPreprocessorExpression(preprocessorToken, currentSymbols, ref tokenIndex);
											if (exp.Evaluate) {
												return;
											}
											break;
									}
								}
							}
						}
					}
					break;
				case PreprocessorType.@else:
				case PreprocessorType.@elif:
					{
						//Find endif
						while (true) {
							var token = GetTokenWorker(infoOutput, source, index);
							if (token == null) {
								return;
							}
							index = token.End;
							if ((token as TokenPreprocessor)?.Type == PreprocessorType.@endif) {
								return;
							}
						}
					}
				case PreprocessorType.@error:
					{
						ParseException exp;
						if (preprocessorToken.ContentTokens.Length > 0) {
							var firstToken = preprocessorToken.ContentTokens[0];
							var lastToken = preprocessorToken.ContentTokens[preprocessorToken.ContentTokens.Length - 1];
							var errorText = preprocessorToken.SourceFile.Substring(firstToken.Begin, lastToken.End);
							exp = ParseException.AsToken(preprocessorToken, "#error: " + errorText);
						}
						else {
							exp = ParseException.AsToken(preprocessorToken, "#error");
						}
						throw exp;
					}
				case PreprocessorType.@warning: 
					{
						if (preprocessorToken.ContentTokens.Length > 0) {
							var firstToken = preprocessorToken.ContentTokens[0];
							var lastToken = preprocessorToken.ContentTokens[preprocessorToken.ContentTokens.Length - 1];
							var errorText = preprocessorToken.SourceFile.Substring(firstToken.Begin, lastToken.End);
							infoOutput.OutputWarning(ParseException.AsToken(preprocessorToken, "#error: " + errorText));
						}
						else {
							infoOutput.OutputWarning(ParseException.AsToken(preprocessorToken, "#error"));
						}
					}
					break;
				case PreprocessorType.@line:
					infoOutput.OutputWarning(ParseException.AsToken(preprocessorToken, Error.Tokenizer.PreprocessorLineNotSupported));
					break;
				case PreprocessorType.@endif:
				case PreprocessorType.@region:
				case PreprocessorType.@endregion:
				case PreprocessorType.@pragma:
					//Ignore these
					break;
				default:
					throw ParseException.AsToken(preprocessorToken, Error.Tokenizer.InvalidPreprocessor);
			}
		}

		static IBooleanExpression ReadPreprocessorExpression(TokenPreprocessor source, SortedSet<string> currentSymbols, ref int tokenIndex)
		{
			var token = GetNextPreprocessorToken(source, ref tokenIndex);
			if (token == null) {
				throw ParseException.AsToken(source, Error.Tokenizer.InvalidPreprocessor);
			}

			//Expression
			IBooleanExpression expLeft;
			var symbolToken = token as TokenSymbol;
			if (symbolToken != null && symbolToken.Type == SymbolType.LeftParenthesis)
			{
				expLeft = ReadPreprocessorExpression(source, currentSymbols, ref tokenIndex);
				token = GetNextPreprocessorToken(source, ref tokenIndex);
				if (token == null || (token as TokenSymbol)?.Type != SymbolType.RightParenthesis) {
					throw ParseException.AsToken(source, Error.Tokenizer.InvalidPreprocessor);
				}
				return expLeft;
			}
			else if (symbolToken != null && symbolToken.Type == SymbolType.LogicalNot) {
				expLeft = ReadPreprocessorExpression(source, currentSymbols, ref tokenIndex);
				return new NegateStatement(expLeft);
			}
			else if (token is TokenBooleanLiteral) {
				expLeft = new BoolKeywordStatement((TokenBooleanLiteral)token);
			}
			else if (token is TokenIdentifier) {
				expLeft = new SymbolStatement((TokenIdentifier)token, currentSymbols);
			}
			else {
				throw ParseException.AsToken(source, Error.Tokenizer.InvalidPreprocessor);
			}

			//Operator
			token = GetNextPreprocessorToken(source, ref tokenIndex);
			symbolToken = token as TokenSymbol;
			if (token == null) {
				return expLeft;
			}
			else if (symbolToken == null) {
				throw ParseException.AsToken(source, Error.Tokenizer.InvalidPreprocessor);
			}
			else {
				switch (symbolToken.Type) {
					case SymbolType.RightParenthesis:
						return expLeft;
					case SymbolType.LogicalAnd:
					case SymbolType.LogicalOr:
						{
							var expRight = ReadPreprocessorExpression(source, currentSymbols, ref tokenIndex);
							return new ConditionalExpression(expLeft, symbolToken.Type == SymbolType.LogicalAnd, expRight);
						}
					case SymbolType.Equal:
					case SymbolType.NotEqual:
						{
							var expRight = ReadPreprocessorExpression(source, currentSymbols, ref tokenIndex);
							return new EqualityExpression(expLeft, symbolToken.Type == SymbolType.Equal, expRight);
						}
					default:
						throw ParseException.AsToken(source, Error.Tokenizer.InvalidPreprocessor);
				}
			}
		}

		static Token GetNextPreprocessorToken(TokenPreprocessor token, ref int index)
		{
			return index < token.ContentTokens.Length ? token.ContentTokens[index++] : null;
		}

#endregion

#region Utility - Number

		delegate bool NumberTestDelegate(char value);

		static bool IsNumber(char value)
		{
			return value >= '0' && value <= '9';
		}

		static bool IsHexNumber(char value)
		{
			return (value >= '0' && value <= '9') ||
				   (value >= 'A' && value <= 'F') ||
				   (value >= 'a' && value <= 'f');
		}

		static bool IsBinaryNumber(char value)
		{
			return value == '0' || value == '1';
		}

		static bool IsOctalNumber(char value)
		{
			return value >= '0' && value <= '7';
		}

		static string ReadNumberText(SourceTextFile source, ref int index, NumberTestDelegate fnNumberTest)
		{
			var numberText = new StringBuilder();
			while (fnNumberTest(source[index])) {
				numberText.Append(source[index]);
				++index;
				if (index >= source.Length) {
					break;
				}

				//Skip digit separator
				if (source[index] == '\'' || source[index] == '_') {
					if (source[index] == '_') {
						while (source[index] == '_') {
							++index;
						}
					}
					else {
						++index;
					}
					if (!fnNumberTest(source[index])) {
						throw ParseException.AsIndex(source, index, Error.Tokenizer.UnknownToken);
					}
				}
			}
			return numberText.ToString();
		}

		delegate bool NumberConvertDelegate(string text, out ulong value);

		static bool ConvertHexNumberString(string text, out ulong value)
		{
			return ulong.TryParse(text, NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out value);
		}

		static bool ConvertBinaryNumberString(string text, out ulong value)
		{
			try {
				value = Convert.ToUInt64(text, 2);
				return true;
			}
			catch (FormatException) {
				value = 0;
				return false;
			}
		}

		static bool ConvertOctalNumberString(string text, out ulong value)
		{
			try {
				value = Convert.ToUInt64(text, 8);
				return true;
			}
			catch (FormatException) {
				value = 0;
				return false;
			}
		}

		enum IntegerTrailingType
		{
			None,
			L,
			U,
			UL,
		}

		static IntegerTrailingType ReadIntegerLiteralTrailing(SourceTextFile source, ref int index)
		{
			int testCount = 0;
			if (index + 1 < source.Length) {
				testCount = 2;
			}
			else if (index < source.Length) {
				testCount = 1;
			}

			if (testCount == 2) {
				if ((char.ToUpper(source[index]) == 'U' && char.ToUpper(source[index + 1]) == 'L') ||
					(char.ToUpper(source[index]) == 'L' && char.ToUpper(source[index + 1]) == 'U')) {
					index += 2;
					return IntegerTrailingType.UL;
				}
			}
			if (testCount >= 1) {
				switch (char.ToUpper(source[index])) {
					case 'U':
						++index;
						return IntegerTrailingType.U;
					case 'L':
						++index;
						return IntegerTrailingType.L;
				}
			}

			return IntegerTrailingType.None;
		}

#endregion

#region Utility - Character

		static char ReadCharacterLiterialEscapeSequence(InfoOutput infoOutput, SourceTextFile source, int firstIndex, ref int index)
		{
			switch (source[index]) {
				case 'u':
					{
						++index;
						int numberStart = index;
						for (int cnt = 0; cnt < 4; ++cnt) {
							if (!IsHexNumber(source[index])) {
								infoOutput.OutputError(ParseException.AsRange(source, firstIndex, index, Error.Tokenizer.UnknownUnicodeCharacter));
								return (char)0;
							}
							++index;
						}
						string hexDigitText = source.Substring(numberStart, index);

						int number;
						if (!int.TryParse(hexDigitText, NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out number)) {
							infoOutput.OutputError(ParseException.AsRange(source, firstIndex, index, Error.Tokenizer.UnknownUnicodeCharacter));
						}
						return (char)number;
					}
				case 'x':
					{
						++index;
						int numberStart = index;
						int numberCount = 0;
						while (IsHexNumber(source[index]) && numberCount < 4) {
							++index;
						}
						string hexDigitText = source.Substring(numberStart, index);

						int number;
						if (!int.TryParse(hexDigitText, NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out number)) {
							infoOutput.OutputError(ParseException.AsRange(source, firstIndex, index, Error.Tokenizer.UnknownUnicodeCharacter));
						}
						return (char)number;
					}
				case '\'':
					++index;
					return '\'';
				case '\"':
					++index;
					return '\"';
				case '\\':
					++index;
					return '\\';
				case '0':
					++index;
					return '\0';
				case 'a':
					++index;
					return '\a';
				case 'b':
					++index;
					return '\b';
				case 'f':
					++index;
					return '\f';
				case 'n':
					++index;
					return '\n';
				case 'r':
					++index;
					return '\r';
				case 't':
					++index;
					return '\t';
				case 'v':
					++index;
					return '\v';
				default:
					infoOutput.OutputWarning(ParseException.AsIndex(source, index, Error.Tokenizer.UnknownEscapeSequence));
					++index;
					return source[index];
			}
		}

#endregion

#region Utility - Identifier

		static bool IsValidIdentifierCharacter(char value)
		{
			return (value >= 'A' && value <= 'Z') ||
				   (value >= 'a' && value <= 'z') ||
				   (value >= '0' && value <= '9') ||
				   (value >= '_');
		}

#endregion
	}
}
