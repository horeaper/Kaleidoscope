using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.SyntaxObject
{
	public class TokenItem<T1, T2> where T2 : Token
	{
		public readonly T1 Item;
		public readonly T2 Token;

		public TokenItem(T1 item, T2 token)
		{
			Item = item;
			Token = token;
		}
	}
}
