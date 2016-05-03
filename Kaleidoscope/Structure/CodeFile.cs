using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.Analysis;

namespace Kaleidoscope.Structure
{
	public class CodeFile
	{
		public readonly TokenBlob Tokens;

		public CodeFile(TokenBlob tokens)
		{
			Tokens = tokens;
		}

		public override string ToString()
		{
			return "[CodeFile] " + Tokens.SourceFile.FileName;
		}
	}
}
