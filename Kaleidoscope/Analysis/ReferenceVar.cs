using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class ReferenceVar : ReferenceToType
	{
		public ReferenceVar(Token token)
			: base(new TokenBlock(token))
		{
		}
	}
}
