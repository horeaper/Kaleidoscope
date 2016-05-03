using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.Analysis;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.SyntaxObject
{
	/// <summary>
	/// using System.Collections.Generic;
	/// </summary>
	class UsingCSNamespaceDirective
	{
		public readonly CodeFile SourceFile;
		public readonly Token[] Namespace;
		public readonly string DisplayName;

		UsingCSNamespaceDirective(CodeFile sourceFile, Token[] nsTokens, string displayName)
		{
			SourceFile = sourceFile;
			Namespace = nsTokens;
			DisplayName = displayName;
		}

		public override string ToString()
		{
			return "[UsingCSNamespaceDirective] using " + DisplayName;
		}
	}
}
