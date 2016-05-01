using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;
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

			//Hex/Binary/Octal Number
			if (source[index] == '0') {
				if (index + 1 >= source.Length) {
					return new TokenSignedNumber(source, index, index + 1, 0, IntegerNumberType.Int);
				}

				//Test for hex number
				if (source[index + 1] == 'x' || source[index + 1] == 'X') {
					return ParseSpecialNumber(source, index, IsHexNumber, ConvertHexNumberString);
				}

				//Test for binary number
				if (source[index + 1] == 'b' || source[index + 1] == 'b') {
					return ParseSpecialNumber(source, index, IsBinaryNumber, ConvertBinaryNumberString);
				}

				//Test for octal number
				if (source[index + 1] == 'o' || source[index + 1] == 'O') {
					return ParseSpecialNumber(source, index, IsOctalNumber, ConvertOctalNumberString);
				}
			}

			//Normal number
			if (source[index] == '-' || IsNumber(source[index])) {
// 				bool isNegative = false;
// 				if (source[index] == '-') {
// 					isNegative = true;
// 					++index;
// 					TestEOF(source, index);
// 				}
			}

			throw ParseException.AsIndex(source, index, Error.Tokenizer.UnknownToken);
		}

#region Common functor

		delegate bool ConvertDelegate(string text, out ulong value);

		static Token ParseSpecialNumber(SourceTextFile source, int index, Func<char, bool> fnNumberTest, ConvertDelegate fnConvertText)
		{
			int startIndex = index;
			index += 2;
			TestEOF(source, index);
			if (!fnNumberTest(source[index])) {
				throw ParseException.AsIndex(source, index, Error.Tokenizer.UnknownToken);
			}

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
					TestEOF(source, index);
					if (!fnNumberTest(source[index])) {
						throw ParseException.AsIndex(source, index, Error.Tokenizer.UnknownToken);
					}
				}
			}

			int trailing = ReadNumberLiteralTrailing(source, ref index);
			switch (trailing) {
				case 3:
					{
						ulong value;
						if (!fnConvertText(numberText.ToString(), out value)) {
							throw ParseException.AsString(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant);
						}
						return new TokenUnsignedNumber(source, startIndex, index, value, IntegerNumberType.Long);
					}
				case 2:
					{
						ulong value;
						if (!fnConvertText(numberText.ToString(), out value)) {
							throw ParseException.AsString(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant);
						}

						if (value > uint.MaxValue) {
							return new TokenUnsignedNumber(source, startIndex, index, value, IntegerNumberType.Long);
						}
						else {
							return new TokenUnsignedNumber(source, startIndex, index, (uint)value, IntegerNumberType.Int);
						}
					}
				case 1:
					{
						ulong value;
						if (!fnConvertText(numberText.ToString(), out value)) {
							throw ParseException.AsString(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant);
						}

						if (value > long.MaxValue) {
							return new TokenUnsignedNumber(source, startIndex, index, value, IntegerNumberType.Long);
						}
						else {
							return new TokenSignedNumber(source, startIndex, index, (long)value, IntegerNumberType.Long);
						}
					}
				default:
					{
						ulong value;
						if (!fnConvertText(numberText.ToString(), out value)) {
							throw ParseException.AsString(source, startIndex, index, Error.Tokenizer.InvalidIntegralConstant);
						}

						if (value > long.MaxValue) {
							return new TokenUnsignedNumber(source, startIndex, index, value, IntegerNumberType.Long);
						}
						else if (value > uint.MaxValue) {
							return new TokenSignedNumber(source, startIndex, index, (long)value, IntegerNumberType.Long);
						}
						else if (value > int.MaxValue) {
							return new TokenUnsignedNumber(source, startIndex, index, (uint)value, IntegerNumberType.Int);
						}
						else {
							return new TokenSignedNumber(source, startIndex, index, (int)value, IntegerNumberType.Int);
						}
					}
			}
		}

#endregion

#region Utils

		static void TestEOF(SourceTextFile source, int index)
		{
			if (index >= source.Length) {
				throw ParseException.AsEOF(source, Error.Tokenizer.UnexpectedEOF);
			}
		}

		static char GetNextChar(SourceTextFile source, int index)
		{
			if (index + 1 < source.Length) {
				return source[index];
			}
			else {
				throw ParseException.AsIndex(source, index, Error.Tokenizer.UnexpectedEOF);
			}
		}

#endregion

#region Utils - Number

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

		//0 - none, 1 = L, 2 = U, 3 = UL/LU
		static int ReadNumberLiteralTrailing(SourceTextFile source, ref int index)
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
					return 3;
				}
			}
			if (testCount >= 1) {
				switch (char.ToUpper(source[index])) {
					case 'U':
						++index;
						return 2;
					case 'L':
						++index;
						return 1;
				}
			}

			return 0;
		}

#endregion
	}
}
