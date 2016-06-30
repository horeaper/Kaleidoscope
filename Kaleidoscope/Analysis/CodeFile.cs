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

		public IEnumerable<RootInstanceTypeDeclare> DefinedTypes => DefinedClasses.Cast<RootInstanceTypeDeclare>()
																				  .Concat(DefinedInterfaces)
																				  .Concat(DefinedEnums)
																				  .Concat(DefinedDelegates);

		public CodeFile(InfoOutput infoOutput, TokenBlock tokens, LanguageType languageType)
		{
			if (infoOutput == null) {
				throw new ArgumentNullException(nameof(infoOutput));
			}
			AnalysisCodeFile analysis;
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

		public void BindUsing(InfoOutput infoOutput, DeclaredNamespaceOrTypeName rootNamespace)
		{
			foreach (var type in DefinedTypes) {
				foreach (var item in type.Usings.UsingCSNamespaceDirectives) {
					item.BindNamespace(infoOutput, rootNamespace);
				}
				foreach (var item in type.Usings.UsingStaticDirectives) {
					item.Type.Bind(new BindContext(infoOutput, rootNamespace, type.Usings.Enclosing, item.OwnerNamespace, new ClassTypeDeclare[0], null));
				}
				foreach (var item in type.Usings.UsingCSAliasDirectives) {
					item.Type.Bind(new BindContext(infoOutput, rootNamespace, type.Usings.Enclosing, item.OwnerNamespace, new ClassTypeDeclare[0], null));
				}

				foreach (var item in type.Usings.UsingCppNamespaceDirectives) {
					item.BindNamespace();
				}
				foreach (var item in type.Usings.UsingCppAliasDirectives) {
					item.Type.Bind(new BindContext(infoOutput, null, null, null, null, null));
				}
			}
		}

		public void BindParents(InfoOutput infoOutput, DeclaredNamespaceOrTypeName rootNamespace)
		{
			foreach (var type in DefinedInterfaces) {
				type.Type.BindParents(infoOutput, rootNamespace, type.Usings, type.Namespace, new Stack<ClassTypeDeclare>());
			}
			foreach (var type in DefinedClasses) {
				type.Type.BindParents(infoOutput, rootNamespace, type.Usings, type.Namespace, new Stack<ClassTypeDeclare>());
			}
		}

		public void BindTypes(InfoOutput infoOutput, DeclaredNamespaceOrTypeName rootNamespace)
		{
			throw new NotImplementedException();
		}
	}
}
