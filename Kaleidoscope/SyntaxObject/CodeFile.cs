using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.Analysis;
using Kaleidoscope.Analysis.CS;
using Kaleidoscope.SyntaxObject.Primitive;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.SyntaxObject
{
	public class CodeFile
	{
		public readonly TokenBlock Tokens;

		public readonly ImmutableArray<UsingCSNamespaceDirective> UsingCSNamespaceDirectives;

		public CodeFile(CodeHub hub, TokenBlock tokens, LanguageType languageType)
		{
			CodeFileAnalysis analysis = null;
			switch (languageType) {
				case LanguageType.CS:
					analysis = new CodeFileAnalysisCS(hub, this, tokens);
					break;
			}
			if (analysis == null) {
				throw new ArgumentException("invalid language type", nameof(languageType));
			}

			Tokens = tokens;
			UsingCSNamespaceDirectives = ImmutableArray.Create(analysis.UsingCSNamespaceDirectives.ToArray());
		}

		public override string ToString()
		{
			return "[CodeFile] " + Tokens.SourceFile.FileName;
		}
	}
}
