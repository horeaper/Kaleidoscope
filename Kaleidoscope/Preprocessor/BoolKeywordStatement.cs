using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Preprocessor
{
	public class BoolKeywordStatement : IBooleanExpression
	{
		public BoolKeywordStatement(TokenBooleanLiteral token)
		{
			Evaluate = token.Value;
		}

		public bool Evaluate { get; }
	}
}
