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
		public readonly ImmutableArray<RootTypeDeclare<ClassTypeDeclare>> DefinedClasses;
		public readonly ImmutableArray<RootTypeDeclare<InterfaceTypeDeclare>> DefinedInterfaces;
		public readonly ImmutableArray<RootTypeDeclare<EnumTypeDeclare>> DefinedEnums;
		public readonly ImmutableArray<RootTypeDeclare<DelegateTypeDeclare>> DefinedDelegates;

		public IEnumerable<InstanceTypeDeclare> DefinedTypes => DefinedClasses.Select(item => item.Type).Cast<InstanceTypeDeclare>()
																			  .Concat(DefinedInterfaces.Select(item => item.Type))
																			  .Concat(DefinedEnums.Select(item => item.Type))
																			  .Concat(DefinedDelegates.Select(item => item.Type));

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
			DefinedClasses = ImmutableArray.CreateRange(analysis.DefinedClasses.Select(item => new RootTypeDeclare<ClassTypeDeclare>(item)));
			DefinedInterfaces = ImmutableArray.CreateRange(analysis.DefinedInterfaces.Select(item => new RootTypeDeclare<InterfaceTypeDeclare>(item)));
			DefinedEnums = ImmutableArray.CreateRange(analysis.DefinedEnums.Select(item => new RootTypeDeclare<EnumTypeDeclare>(item)));
			DefinedDelegates = ImmutableArray.CreateRange(analysis.DefinedDelegates.Select(item => new RootTypeDeclare<DelegateTypeDeclare>(item)));
		}

		public override string ToString()
		{
			return "[CodeFile] " + Tokens.SourceFile.FileName;
		}
	}
}
