using System;
using System.Collections.Generic;
using Kaleidoscope.Analysis.Internal;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class CodeFileAnalysisCS : CodeFileAnalysis
	{
		readonly AnalyzedFile ownerFile;
		readonly TokenBlock block;

		int index;
		readonly UsingStack currentUsings = new UsingStack();
		readonly NamespaceStack currentNamespace = new NamespaceStack();

		public CodeFileAnalysisCS(AnalyzedFile ownerFile, TokenBlock block)
		{
			this.ownerFile = ownerFile;
			this.block = block;
			ReadContent();
		}

		void ReadContent()
		{
			var currentAttributes = new List<AttributeObject>();

			while (true) {
				var token = block.GetToken(index);
				if (token == null) {
					return;
				}

				switch (token.Type) {
					case TokenType.@using:
						{
							if (currentAttributes.Count > 0) {
								throw ParseException.AsTokenBlock(currentAttributes[currentAttributes.Count - 1].Type.Content, Error.Analysis.InvalidAttributeUsage);
							}
							++index;
							token = block.GetToken(index);

							if (token.Type == TokenType.@static) {
								++index;
								var typeContent = block.ReadPastSpecificToken(ref index, TokenType.Semicolon, Error.Analysis.SemicolonExpected);
								currentUsings.Peek().UsingStaticDirectives.Add(UsingReader.ReadStatic(currentNamespace.ToArray(), typeContent));
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
									currentUsings.Peek().UsingCSNamespaceDirectives.Add(UsingReader.ReadCSNamespace(currentNamespace.ToArray(), content));
								}
								else {
									currentUsings.Peek().UsingCppNamespaceDirectives.Add(UsingReader.ReadCppNamespace(content));
								}
							}
							else {
								if (!isCppType) {
									currentUsings.Peek().UsingCSAliasDirectives.Add(UsingReader.ReadCSAlias(currentNamespace.ToArray(), content));
								}
								else {
									currentUsings.Peek().UsingCppAliasDirectives.Add(UsingReader.ReadCppAlias(content));
								}
							}
						}
						break;
					case TokenType.@namespace:
						{
							if (currentAttributes.Count > 0) {
								throw ParseException.AsTokenBlock(currentAttributes[currentAttributes.Count - 1].Type.Content, Error.Analysis.InvalidAttributeUsage);
							}

							++index;
							var ns = UsingReader.ReadNamespace(block, ref index);
							currentNamespace.Push(ns);
							currentUsings.Push();
							ReadContent();
							currentNamespace.Pop();
							currentUsings.Pop();
						}
						break;
					case TokenType.LeftBracket:
						++index;
						currentAttributes.Add(AttributeObjectReader.ReadOnType(block, ref index, currentUsings.Peek(), currentNamespace.ToArray()));
						break;
					case TokenType.Semicolon:
						if (currentAttributes.Count > 0) {
							throw ParseException.AsTokenBlock(currentAttributes[currentAttributes.Count - 1].Type.Content, Error.Analysis.InvalidAttributeUsage);
						}
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

		void ReadNextTypeDeclare(bool isPublic, AttributeObject[] customAttributes)
		{
			TokenKeyword instanceKindModifier = null;
			TokenKeyword unsafeModifier = null;
			TokenIdentifier partialModifier = null;

			while (true) {
				var token = block.GetToken(index++, Error.Analysis.UnexpectedToken);
				switch (token.Type) {
					case TokenType.@public:
						if (isPublic) {
							throw ParseException.AsToken(token, Error.Analysis.DuplicatedModifier);
						}
						else {
							throw ParseException.AsToken(token, Error.Analysis.InconsistentModifierOrder);
						}
					case TokenType.@abstract:
					case TokenType.@sealed:
					case TokenType.@static:
						CheckConflict(instanceKindModifier, token);
						CheckInconsistent(unsafeModifier, partialModifier);
						instanceKindModifier = (TokenKeyword)token;
						break;
					case TokenType.@unsafe:
						CheckConflict(unsafeModifier, token);
						CheckInconsistent(partialModifier);
						unsafeModifier = (TokenKeyword)token;
						break;
					case TokenType.@class:
						ReadRootClassTypeDeclare(customAttributes, isPublic, unsafeModifier != null, partialModifier != null, (TypeInstanceKind)Enum.Parse(typeof(TypeInstanceKind), instanceKindModifier.Type.ToString()), nameToken => ReadClassMembers<RootClassTypeDeclare.Builder>(nameToken, ClassTypeKind.@interface));
						return;
					case TokenType.@struct:
						if (instanceKindModifier != null) {
							throw ParseException.AsToken(instanceKindModifier, Error.Analysis.InvalidModifier);
						}
						ReadRootClassTypeDeclare(customAttributes, isPublic, unsafeModifier != null, partialModifier != null, TypeInstanceKind.None, nameToken => ReadClassMembers<RootClassTypeDeclare.Builder>(nameToken, ClassTypeKind.@struct));
						return;
					case TokenType.@interface:
						if (instanceKindModifier != null) {
							throw ParseException.AsToken(instanceKindModifier, Error.Analysis.InvalidModifier);
						}
						ReadRootClassTypeDeclare(customAttributes, isPublic, unsafeModifier != null, partialModifier != null, TypeInstanceKind.None, ReadInterfaceMembers<RootClassTypeDeclare.Builder>);
						return;
					case TokenType.@enum:
						if (instanceKindModifier != null) {
							throw ParseException.AsToken(instanceKindModifier, Error.Analysis.InvalidModifier);
						}
						if (unsafeModifier != null) {
							throw ParseException.AsToken(unsafeModifier, Error.Analysis.InvalidModifier);
						}
						if (partialModifier != null) {
							throw ParseException.AsToken(partialModifier, Error.Analysis.PartialWithClassOnly);
						}
						ReadEnum(customAttributes, isPublic);
						return;
					case TokenType.@delegate:
						if (instanceKindModifier != null) {
							throw ParseException.AsToken(instanceKindModifier, Error.Analysis.InvalidModifier);
						}
						if (unsafeModifier != null) {
							throw ParseException.AsToken(unsafeModifier, Error.Analysis.InvalidModifier);
						}
						if (partialModifier != null) {
							throw ParseException.AsToken(partialModifier, Error.Analysis.PartialWithClassOnly);
						}
						ReadDelegate(customAttributes, isPublic);
						return;
					default:
						{
							var identifierToken = token as TokenIdentifier;
							switch (identifierToken?.ContextualKeyword) {
								case ContextualKeywordType.partial:
									partialModifier = identifierToken;
									break;
								case ContextualKeywordType.inline:
									throw ParseException.AsToken(token, Error.Analysis.InlineNotAllowed);
								default:
									throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
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
