using System.Collections.Generic;
using Kaleidoscope.Analysis.Internal;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class CodeFileAnalysisCS : CodeFileAnalysis
	{
		IInfoOutput infoOutput;
		AnalyzedFile codeFile;
		TokenBlock block;

		int index;
		UsingBlob.Builder currentUsings = new UsingBlob.Builder();
		NamespaceStack currentNamespace = new NamespaceStack();

		public CodeFileAnalysisCS(IInfoOutput infoOutput, AnalyzedFile codeFile, TokenBlock block)
		{
			this.infoOutput = infoOutput;
			this.codeFile = codeFile;
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
								currentUsings.UsingStaticDirectives.Add(UsingReader.ReadStatic(currentNamespace.ToArray(), typeContent));
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
									currentUsings.UsingCSNamespaceDirectives.Add(UsingReader.ReadCSNamespace(currentNamespace.ToArray(), content));
								}
								else {
									currentUsings.UsingCppNamespaceDirectives.Add(UsingReader.ReadCppNamespace(currentNamespace.ToArray(), content));
								}
							}
							else {
								if (!isCppType) {
									currentUsings.UsingCSAliasDirectives.Add(UsingReader.ReadCSAlias(currentNamespace.ToArray(), content));
								}
								else {
									currentUsings.UsingCppAliasDirectives.Add(UsingReader.ReadCppAlias(currentNamespace.ToArray(), content));
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
							ReadContent();
							currentNamespace.Pop();
						}
						break;
					case TokenType.Semicolon:
						{
							if (currentAttributes.Count > 0) {
								throw ParseException.AsTokenBlock(currentAttributes[currentAttributes.Count - 1].Type.Content, Error.Analysis.InvalidAttributeUsage);
							}
							++index;
						}
						break;
					case TokenType.LeftBracket:
						{
							++index;
							var type = (ReferenceToManagedType)TypeReferenceReader.Read(block, ref index, TypeParsingRule.None);

							TokenBlock content = null;
							token = block.GetToken(index, Error.Analysis.RightBracketExpected);
							if (token.Type == TokenType.LeftParenthesis) {
								int startIndex = index + 1;
								int endIndex = block.FindNextParenthesisBlockEnd(index);
								content = block.AsBeginEnd(startIndex, endIndex - 1);

								index = endIndex;
								token = block.GetToken(index++, Error.Analysis.RightBracketExpected);
							}
							if (token.Type != TokenType.RightBracket) {
								throw ParseException.AsToken(token, Error.Analysis.RightBracketExpected);
							}

							currentAttributes.Add(new AttributeObject(new UsingBlob(currentUsings), currentNamespace.ToArray(), type, content));
						}
						break;
					case TokenType.@public:
						++index;
						ReadNextClassModifier(true, currentAttributes.ToArray());
						currentAttributes.Clear();
						break;
					case TokenType.@abstract:
					case TokenType.@static:
					case TokenType.@sealed:
					case TokenType.@unsafe:
						ReadNextClassModifier(false, currentAttributes.ToArray());
						currentAttributes.Clear();
						break;
					case TokenType.@class:
						++index;
						ReadClass(currentAttributes.ToArray(), false, null, false);
						currentAttributes.Clear();
						break;
					case TokenType.@struct:
						++index;
						ReadStruct(currentAttributes.ToArray(), false, false);
						currentAttributes.Clear();
						break;
					case TokenType.@interface:
						++index;
						ReadInterface(currentAttributes.ToArray(), false);
						currentAttributes.Clear();
						break;
					case TokenType.@enum:
						++index;
						ReadEnum(currentAttributes.ToArray(), false);
						currentAttributes.Clear();
						break;
					case TokenType.@delegate:
						++index;
						ReadDelegate(currentAttributes.ToArray(), false);
						currentAttributes.Clear();
						break;
					default:
						if ((token as TokenIdentifier).ContextualKeyword == ContextualKeywordType.inline) {
							throw ParseException.AsToken(token, Error.Analysis.InlineNotAllowed);
						}
						else {
							throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
						}
				}
			}
		}

		void ReadNextClassModifier(bool isPublic, AttributeObject[] customAttributes)
		{
			TokenKeyword typeModifier = null;
			TokenKeyword unsafeModifier = null;

			while (true) {
				var token = block.GetToken(index, Error.Analysis.UnexpectedToken);

				switch (token.Type) {
					case TokenType.@public:
						if (isPublic) {
							throw ParseException.AsToken(token, Error.Analysis.DuplicatedModifier);
						}
						else {
							throw ParseException.AsToken(token, Error.Analysis.InconsistentModifierOrder);
						}
					case TokenType.@abstract:
					case TokenType.@static:
					case TokenType.@sealed:
						if (typeModifier != null) {
							throw ParseException.AsToken(token, Error.Analysis.ConflictModifier);
						}
						if (unsafeModifier != null) {
							throw ParseException.AsToken(token, Error.Analysis.InconsistentModifierOrder);
						}
						typeModifier = (TokenKeyword)token;
						++index;
						break;
					case TokenType.@unsafe:
						if (unsafeModifier != null) {
							throw ParseException.AsToken(token, Error.Analysis.DuplicatedModifier);
						}
						unsafeModifier = (TokenKeyword)token;
						++index;
						break;
					case TokenType.@class:
						++index;
						ReadClass(customAttributes, isPublic, typeModifier, unsafeModifier != null);
						return;
					case TokenType.@struct:
						if (typeModifier != null) {
							throw ParseException.AsToken(typeModifier, Error.Analysis.InvalidModifier);
						}
						++index;
						ReadStruct(customAttributes, isPublic, unsafeModifier != null);
						return;
					case TokenType.@interface:
						if (typeModifier != null) {
							throw ParseException.AsToken(typeModifier, Error.Analysis.InvalidModifier);
						}
						if (unsafeModifier != null) {
							throw ParseException.AsToken(unsafeModifier, Error.Analysis.InvalidModifier);
						}
						++index;
						ReadInterface(customAttributes, isPublic);
						return;
					case TokenType.@enum:
						if (typeModifier != null) {
							throw ParseException.AsToken(typeModifier, Error.Analysis.InvalidModifier);
						}
						if (unsafeModifier != null) {
							throw ParseException.AsToken(unsafeModifier, Error.Analysis.InvalidModifier);
						}
						++index;
						ReadEnum(customAttributes, isPublic);
						return;
					case TokenType.@delegate:
						if (typeModifier != null) {
							throw ParseException.AsToken(typeModifier, Error.Analysis.InvalidModifier);
						}
						if (unsafeModifier != null) {
							throw ParseException.AsToken(unsafeModifier, Error.Analysis.InvalidModifier);
						}
						++index;
						ReadDelegate(customAttributes, isPublic);
						return;
					default:
						if ((token as TokenIdentifier).ContextualKeyword == ContextualKeywordType.inline) {
							throw ParseException.AsToken(token, Error.Analysis.InlineNotAllowed);
						}
						else {
							throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
						}
				}
			}
		}
	}
}
