using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	public static class UsingReader
	{
		public static IEnumerable<TokenIdentifier> ReadNamespace(TokenBlock block, ref int index)
		{
			return ReadNamespaceWorker(block, ref index, token => token.Type == TokenType.Identifier).Cast<TokenIdentifier>();
		}

		static IEnumerable<Token> ReadNamespace(TokenBlock block, Func<Token, bool> fnCheckTokenType)
		{
			int index = 0;
			return ReadNamespaceWorker(block, ref index, fnCheckTokenType);
		}

		static IEnumerable<Token> ReadNamespaceWorker(TokenBlock block, ref int index, Func<Token, bool> fnCheckTokenType)
		{
			var ns = new List<Token>();
			while (true) {
				var token = block.GetToken(index++, Error.Analysis.UnexpectedToken);
				if (fnCheckTokenType(token)) {
					ns.Add(token);

					token = block.GetToken(index);
					if (token == null || token.Type == TokenType.LeftBrace) {
						return ns;
					}
					else if (token.Type == TokenType.Dot) {
						++index;
					}
					else {
						throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
					}
				}
				else {
					throw ParseException.AsToken(token, Error.Analysis.IdentifierExpected);
				}
			}
		}

		public static UsingStaticDirective ReadStatic(ImmutableArray<TokenIdentifier>.Builder ownerNamespace, TokenBlock block)
		{
			int index = 0;
			return new UsingStaticDirective(ownerNamespace, (ReferenceToManagedType)TypeReferenceReader.Read(block, ref index, TypeParsingRule.None));
		}

		public static UsingCSNamespaceDirective ReadCSNamespace(ImmutableArray<TokenIdentifier>.Builder ownerNamespace, TokenBlock block)
		{
			return new UsingCSNamespaceDirective(ownerNamespace, ReadNamespace(block, token => token.Type == TokenType.Identifier).Cast<TokenIdentifier>().ToArray());
		}

		public static UsingCppNamespaceDirective ReadCppNamespace(TokenBlock block)
		{
			return new UsingCppNamespaceDirective(ReadNamespace(block, token => token is TokenKeyword || token.Type == TokenType.Identifier));
		}

		public static UsingCSAliasDirective ReadCSAlias(ImmutableArray<TokenIdentifier>.Builder ownerNamespace, TokenBlock block)
		{
			int index = 0;

			var token = block.GetToken(index++, Error.Analysis.UnexpectedToken);
			if (token.Type != TokenType.Identifier) {
				throw ParseException.AsToken(token, Error.Analysis.IdentifierExpected);
			}
			var nameToken = token;

			token = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (token.Type != TokenType.Assign) {
				throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
			}

			var refToType = (ReferenceToManagedType)TypeReferenceReader.Read(block, ref index, TypeParsingRule.None);
			return new UsingCSAliasDirective(ownerNamespace, nameToken, refToType);
		}

		public static UsingCppAliasDirective ReadCppAlias(TokenBlock block)
		{
			int index = 0;

			var token = block.GetToken(index++, Error.Analysis.UnexpectedToken);
			if (token.Type != TokenType.Identifier) {
				throw ParseException.AsToken(token, Error.Analysis.IdentifierExpected);
			}
			var nameToken = token;

			token = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (token.Type != TokenType.Assign) {
				throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
			}

			int startIndex = index;
			token = block.GetToken(index++, Error.Analysis.MissingCppKeyword);
			if ((token as TokenIdentifier)?.ContextualKeyword != ContextualKeywordType.cpp) {
				throw ParseException.AsToken(token, Error.Analysis.MissingCppKeyword);
			}
			token = block.GetToken(index, Error.Analysis.MissingCppKeyword);
			if (token.Type != TokenType.DoubleColon) {
				throw ParseException.AsToken(token, Error.Analysis.MissingCppKeyword);
			}

			var refToType = (ReferenceToCppType)TypeReferenceReader.Read(block, ref startIndex, TypeParsingRule.AllowCppType);
			return new UsingCppAliasDirective(nameToken, refToType);
		}
	}
}
