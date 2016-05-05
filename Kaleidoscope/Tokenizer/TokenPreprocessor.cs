using System;

namespace Kaleidoscope.Tokenizer
{
	public class TokenPreprocessor : Token
	{
		public new readonly PreprocessorType Type;
		public readonly Token[] ContentTokens;

		public TokenPreprocessor(SourceTextFile sourceFile, int begin, int end, PreprocessorType type, Token[] contentTokens)
			: base(sourceFile, begin, end, TokenType.Preprocessor)
		{
			Type = type;
			ContentTokens = contentTokens;
		}

		public static bool IsPreprocessor(string text)
		{
			PreprocessorType tmp;
			return Enum.TryParse(text, out tmp);
		}
	}
}
