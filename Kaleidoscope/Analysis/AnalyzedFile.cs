﻿using System;
using Kaleidoscope.Analysis.CS;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public class AnalyzedFile
	{
		public readonly TokenBlock Tokens;

		public AnalyzedFile(CodeHub hub, TokenBlock tokens, LanguageType languageType)
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
		}

		public override string ToString()
		{
			return "[CodeFile] " + Tokens.SourceFile.FileName;
		}
	}
}