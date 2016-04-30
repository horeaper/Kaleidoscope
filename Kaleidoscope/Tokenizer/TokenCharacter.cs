using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.Primitive;

namespace Kaleidoscope.Tokenizer
{
	public class TokenCharacter : Token
	{
		public readonly char Content;

		public TokenCharacter(SourceTextFile sourceFile, int begin, int end, char content)
			: base(sourceFile, begin, end)
		{
			Content = content;
		}
	}
}
