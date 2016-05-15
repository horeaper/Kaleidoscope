using System;
using Kaleidoscope.Analysis.CS;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class CodeFile
	{
		public readonly TokenBlock Tokens;

		public CodeFile(InfoOutput infoOutput, TokenBlock tokens, LanguageType languageType)
		{
			if (infoOutput == null) {
				throw new ArgumentNullException(nameof(infoOutput));
			}
			AnalysisCodeFile analysis = null;
			switch (languageType) {
				case LanguageType.CS:
					analysis = new AnalysisCodeFileCS(infoOutput, this, tokens);
					break;
				default:
					throw new ArgumentException("invalid language type", nameof(languageType));
			}

			Tokens = tokens;
			//TODO: Set members
		}

		public override string ToString()
		{
			return "[CodeFile] " + Tokens.SourceFile.FileName;
		}
	}
}
