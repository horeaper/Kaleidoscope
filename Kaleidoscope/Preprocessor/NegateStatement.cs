namespace Kaleidoscope.Preprocessor
{
	public class NegateStatement : IBooleanExpression
	{
		public readonly IBooleanExpression Expression;

		public NegateStatement(IBooleanExpression expression)
		{
			Expression = expression;
		}

		public bool Evaluate => !Expression.Evaluate;
	}
}
