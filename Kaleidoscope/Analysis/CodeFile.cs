using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public class CodeFile
	{
		public readonly TokenBlock Tokens;

		public CodeFile(CodeHub codeHub, TokenBlock tokenBlock)
		{
			Tokens = tokenBlock;


		}

		public override string ToString()
		{
			return "[CodeFile] " + Tokens.SourceFile.FileName;
		}
	}
}
