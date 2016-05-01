using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Kaleidoscope.Primitive;

namespace Kaleidoscope.Tokenizer
{
	public static class Tokenizer
	{
		public static ImmutableArray<Token> Process(SourceTextFile source, IEnumerable<string> definedSymbols)
		{
			var builder = ImmutableArray.CreateBuilder<Token>();

			var currentSymbols = new SortedSet<string>(definedSymbols ?? new string[] { });
			int index = 0;
			while (index < source.Length) {
				var token = GetTokenWorker(source, index);
				if (token == null) {
					break;
				}

				builder.Add(token);
				index += token.Length;
			}

			builder.Capacity = builder.Count;
			return builder.MoveToImmutable();
		}

		static Token GetTokenWorker(SourceTextFile source, int index)
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
										throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant);
									}
									return new TokenUnsignedInteger(source, startIndex, index, value, IntegerNumberType.Long);
								}
							case IntegerTrailingType.U:
								{
									ulong value;
									if (!fnConvertText(numberText, out value)) {
										throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant);
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
										throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant);
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
										throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant);
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
							throw ParseException.AsIndex(source, index, Error.Tokenizer.UnknownToken);
						}
					}
					bool isDotFirst = false;
					if (source[index] == '.') {
						isDotFirst = true;
						++index;
						if (!IsNumber(source[index])) {
							throw ParseException.AsIndex(source, index, Error.Tokenizer.UnknownToken);
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
								throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant);
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
								throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant);
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
							throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant);
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
							throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant);
						}
						if (isMinusFirst && (integerTrailing == IntegerTrailingType.UL || integerTrailing == IntegerTrailingType.U)) {
							throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant);
						}

						switch (integerTrailing) {
							case IntegerTrailingType.UL:
								{
									ulong value;
									if (!ulong.TryParse(numberText, NumberStyles.None, CultureInfo.CurrentCulture, out value)) {
										throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant);
									}
									return new TokenUnsignedInteger(source, startIndex, index, value, IntegerNumberType.Long);
								}
							case IntegerTrailingType.U:
								{
									ulong value;
									if (!ulong.TryParse(numberText, NumberStyles.None, CultureInfo.CurrentCulture, out value)) {
										throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant);
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
											throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant);
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
											throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant);
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
										throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant);
									}
									return new TokenFloatNumber(source, startIndex, index, value, FloatNumberType.Float);
								}
							case 'D':
							case 'd':
								{
									++index;
									double value;
									if (!double.TryParse(numberText, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value)) {
										throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant);
									}
									return new TokenFloatNumber(source, startIndex, index, value, FloatNumberType.Double);
								}
							case 'M':
							case 'm':
								{
									++index;
									decimal value;
									if (!decimal.TryParse(numberText, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value)) {
										throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidDecimalConstant);
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
											throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant);
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
													throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant);
												}
												return new TokenFloatNumber(source, startIndex, index, value, FloatNumberType.Float);
											}
										case 'D':
										case 'd':
											{
												++index;
												double value;
												if (!double.TryParse(numberText, NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value)) {
													throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant);
												}
												return new TokenFloatNumber(source, startIndex, index, value, FloatNumberType.Double);
											}
										case 'M':
										case 'm':
											{
												++index;
												decimal value;
												if (!decimal.TryParse(numberText, NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value)) {
													throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidDecimalConstant);
												}
												return new TokenDecimalNumber(source, startIndex, index, value);
											}
										default:
											{
												double value;
												if (!double.TryParse(numberText, NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value)) {
													throw ParseException.AsRange(source, startIndex, index, Error.Tokenizer.InvalidRealConstant);
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
			}
			catch (IndexOutOfRangeException) {
				throw ParseException.AsEOF(source, Error.Tokenizer.UnexpectedEOF);
			}

			throw ParseException.AsIndex(source, index, Error.Tokenizer.UnknownToken);
		}

#region Utils - Number

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
					++index;
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
	}
}
