namespace Kaleidoscope.Preprocessor
{
	public class ConditionalExpression : IBooleanExpression
	{
		public readonly IBooleanExpression Left;
		public readonly bool IsAnd;
		public readonly IBooleanExpression Right;

		public ConditionalExpression(IBooleanExpression left, bool isAnd, IBooleanExpression right)
		{
			Left = left;
			IsAnd = isAnd;
			Right = right;
		}

		public bool Evaluate => IsAnd ? Left.Evaluate && Right.Evaluate : Left.Evaluate || Right.Evaluate;
	}
}
