namespace Kaleidoscope.Preprocessor
{
	public class EqualityExpression : IBooleanExpression
	{
		public readonly IBooleanExpression Left;
		public readonly bool IsEqual;
		public readonly IBooleanExpression Right;

		public EqualityExpression(IBooleanExpression left, bool isEqual, IBooleanExpression right)
		{
			Left = left;
			IsEqual = isEqual;
			Right = right;
		}

		public bool Evaluate => IsEqual ? Left.Evaluate == Right.Evaluate : Left.Evaluate != Right.Evaluate;
	}
}
