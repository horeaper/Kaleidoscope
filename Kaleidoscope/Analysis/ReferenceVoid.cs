using System.Collections.Generic;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class ReferenceVoid : ReferenceToType
	{
		public ReferenceVoid(Token token)
			: base(new TokenBlock(token))
		{
		}

		internal override void Bind(BindContext context)
		{
		}
	}
}
