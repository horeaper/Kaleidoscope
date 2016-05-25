using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kaleidoscope.Analysis.CS;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class CodeFile
	{
		public readonly TokenBlock Tokens;
		public readonly ImmutableArray<RootClassTypeDeclare> DefinedClasses;
		public readonly ImmutableArray<RootInterfaceTypeDeclare> DefinedInterfaces;

		public IEnumerable<InstanceTypeDeclare> DefinedTypes => DefinedClasses.Cast<InstanceTypeDeclare>().Concat(DefinedInterfaces);

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
			DefinedClasses = ImmutableArray.CreateRange(analysis.DefinedClasses);
			DefinedInterfaces = ImmutableArray.CreateRange(analysis.DefinedInterfaces);
		}

		public override string ToString()
		{
			return "[CodeFile] " + Tokens.SourceFile.FileName;
		}
	}
}
