using System;
using System.Collections.Generic;
using Kaleidoscope.Analysis.Internal;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class AnalysisCodeFileCS : AnalysisCodeFile
	{
		readonly InfoOutput infoOutput;
		readonly CodeFile ownerFile;
		readonly TokenBlock block;

		int index;
		readonly UsingStack currentUsings = new UsingStack();
		readonly NamespaceStack currentNamespace = new NamespaceStack();

		public AnalysisCodeFileCS(InfoOutput infoOutput, CodeFile ownerFile, TokenBlock block)
		{
			this.infoOutput = infoOutput;
			this.ownerFile = ownerFile;
			this.block = block;
			ReadContent();
		}

		void ReadContent()
		{
			var currentAttributes = new List<AttributeObject.Builder>();

			while (true) {
				var token = block.GetToken(index);
				if (token == null || token.Type == TokenType.RightBrace) {
					return;
				}

				switch (token.Type) {
					case TokenType.@using:
						{
							EnsureEmpty(currentAttributes);
							++index;
							token = block.GetToken(index);

							if (token.Type == TokenType.@static) {
								++index;
								var typeContent = block.ReadPastSpecificToken(ref index, TokenType.Semicolon, Error.Analysis.SemicolonExpected);
								currentUsings.Peek().UsingStaticDirectives.Add(UsingReader.ReadStatic(currentNamespace.Get(), typeContent));
								continue;
							}

							bool isCppType = false;
							if (token.Type == TokenType.Identifier && token.Text == "cpp") {
								token = block.GetToken(index + 1, Error.Analysis.SemicolonExpected);
								if (token.Type == TokenType.DoubleColon) {
									isCppType = true;
									index += 2;
								}
							}

							var content = block.ReadPastSpecificToken(ref index, TokenType.Semicolon, Error.Analysis.SemicolonExpected);
							if (content.FindToken(0, TokenType.Assign) == -1) {
								if (!isCppType) {
									currentUsings.Peek().UsingCSNamespaceDirectives.Add(UsingReader.ReadCSNamespace(currentNamespace.Get(), content));
								}
								else {
									currentUsings.Peek().UsingCppNamespaceDirectives.Add(UsingReader.ReadCppNamespace(content));
								}
							}
							else {
								if (!isCppType) {
									currentUsings.Peek().UsingCSAliasDirectives.Add(UsingReader.ReadCSAlias(currentNamespace.Get(), content));
								}
								else {
									currentUsings.Peek().UsingCppAliasDirectives.Add(UsingReader.ReadCppAlias(content));
								}
							}
						}
						break;
					case TokenType.@namespace:
						{
							EnsureEmpty(currentAttributes);
							++index;
							var ns = UsingReader.ReadNamespace(block, ref index);
							block.NextToken(index, TokenType.LeftBrace, Error.Analysis.LeftBraceExpected);
							++index;
							currentNamespace.Push(ns);
							currentUsings.Push();
							ReadContent();
							currentNamespace.Pop();
							currentUsings.Pop();
							block.NextToken(index, TokenType.RightBrace, Error.Analysis.RightBraceExpected);
							++index;
						}
						break;
					case TokenType.LeftBracket:
						++index;
						currentAttributes.Add(AttributeObjectReader.Read(block, ref index));
						break;
					case TokenType.Semicolon:
						EnsureEmpty(currentAttributes);
						++index;
						break;
					case TokenType.@public:
						++index;
						ReadNextTypeDeclare(true, currentAttributes.ToArray());
						currentAttributes.Clear();
						break;
					default:
						ReadNextTypeDeclare(false, currentAttributes.ToArray());
						currentAttributes.Clear();
						break;
				}
			}
		}

		void ReadNextTypeDeclare(bool isPublic, AttributeObject.Builder[] customAttributes)
		{
			TokenKeyword instanceKindModifier = null;
			TokenIdentifier partialModifier = null;

			while (true) {
				var token = block.GetToken(index++, Error.Analysis.UnexpectedToken);
				switch (token.Type) {
					case TokenType.@public:
						if (isPublic) {
							infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.DuplicatedModifier));
						}
						else {
							infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.InconsistentModifierOrder));
							isPublic = true;
						}
						break;
					case TokenType.@abstract:
					case TokenType.@sealed:
					case TokenType.@static:
						CheckConflict(instanceKindModifier, token);
						CheckInconsistent(partialModifier);
						instanceKindModifier = (TokenKeyword)token;
						break;
					case TokenType.@unsafe:
						infoOutput.OutputWarning(ParseException.AsToken(token, Error.Analysis.UnsafeNotAllowed));
						break;
					case TokenType.@class:
						{
							var instanceKind = TypeInstanceKind.None;
							if (instanceKindModifier != null) {
								instanceKind = (TypeInstanceKind)Enum.Parse(typeof(TypeInstanceKind), instanceKindModifier.Type.ToString());
							}
							DefinedClasses.Add(ReadRootClassDeclare(customAttributes, isPublic, partialModifier != null, instanceKind, traits => ReadClassMembers<RootClassTypeDeclare.Builder>(traits, ClassTypeKind.@class)));
						}
						return;
					case TokenType.@struct:
						if (instanceKindModifier != null) {
							infoOutput.OutputError(ParseException.AsToken(instanceKindModifier, Error.Analysis.InvalidModifier));
						}
						DefinedClasses.Add(ReadRootClassDeclare(customAttributes, isPublic, partialModifier != null, TypeInstanceKind.None, traits => ReadClassMembers<RootClassTypeDeclare.Builder>(traits, ClassTypeKind.@struct)));
						return;
					case TokenType.@interface:
						if (instanceKindModifier != null) {
							infoOutput.OutputError(ParseException.AsToken(instanceKindModifier, Error.Analysis.InvalidModifier));
						}
						DefinedInterfaces.Add(ReadRootInterfaceDeclare(customAttributes, isPublic, partialModifier != null));
						return;
					case TokenType.@enum:
						if (instanceKindModifier != null) {
							infoOutput.OutputError(ParseException.AsToken(instanceKindModifier, Error.Analysis.InvalidModifier));
						}
						if (partialModifier != null) {
							infoOutput.OutputError(ParseException.AsToken(partialModifier, Error.Analysis.PartialWithClassOnly));
						}
// 						ReadEnum(customAttributes, isPublic);
						return;
					case TokenType.@delegate:
						if (instanceKindModifier != null) {
							infoOutput.OutputError(ParseException.AsToken(instanceKindModifier, Error.Analysis.InvalidModifier));
						}
						if (partialModifier != null) {
							infoOutput.OutputError(ParseException.AsToken(partialModifier, Error.Analysis.PartialWithClassOnly));
						}
// 						ReadDelegate(customAttributes, isPublic);
						return;
					default:
						{
							var identifierToken = token as TokenIdentifier;
							switch (identifierToken?.ContextualKeyword) {
								case ContextualKeywordType.partial:
									CheckDuplicate(partialModifier, token);
									partialModifier = identifierToken;
									break;
								case ContextualKeywordType.inline:
									infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.InlineNotAllowed));
									break;
								default:
									infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.UnexpectedToken));
									break;
							}
						}
						break;
				}
			}
		}
	}
}

/*
 * public
 * static/abstract/sealed
 * unsafe (with class/struct/interface)
 * partial (with class/struct/interface)
 * class/struct/interface/enum/delegate
 * 
 */
