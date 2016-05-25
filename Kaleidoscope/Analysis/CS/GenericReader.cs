using System;
using System.Collections.Generic;
using System.Linq;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	public static class GenericReader
	{
		public static List<GenericDeclare.Builder> ReadDeclare(TokenBlock block, ref int index, string eofErrorMessage)
		{
			var result = new List<GenericDeclare.Builder>();

			var token = block.GetToken(index, eofErrorMessage);
			if (token.Type != TokenType.LeftArrow) {
				return result;
			}
			++index;

			while (true) {
				token = block.GetToken(index++, Error.Analysis.UnexpectedToken);

				var item = new GenericDeclare.Builder();
				if (token.Type == TokenType.@in) {
					item.IsContravariance = true;
					token = block.GetToken(index++, Error.Analysis.IdentifierExpected);
				}
				else if (token.Type == TokenType.@out) {
					item.IsContravariance = false;
					token = block.GetToken(index++, Error.Analysis.IdentifierExpected);
				}

				if (token.Type == TokenType.Identifier) {
					item.Name = (TokenIdentifier)token;

					//Check duplicate
					if (result.Any(exist => exist.Name.Text == item.Name.Text)) {
						throw ParseException.AsToken(token, Error.Analysis.DuplicatedGenericName);
					}
					result.Add(item);

					//Next
					token = block.GetToken(index++, Error.Analysis.RightArrowExpected);
					if (token.Type == TokenType.RightArrow) {
						break;
					}
					else if (token.Type != TokenType.Comma) {
						throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
					}
				}
				else {
					throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
				}
			}

			return result;
		}

		public static void ReadConstraint(IEnumerable<GenericDeclare.Builder> generics, TokenBlock block, ref int index, string eofErrorMessage)   //where T : IDisposable, IComparable<T>
		{
			while (true) {
				//where
				var token = block.GetToken(index, eofErrorMessage);
				if ((token as TokenIdentifier)?.ContextualKeyword != ContextualKeywordType.where) {
					return;
				}
				++index;

				//name
				var nameToken = block.GetToken(index++, Error.Analysis.UnexpectedToken);
				var targetItem = generics.FirstOrDefault(item => item.Name.Text == nameToken.Text);
				if (targetItem == null) {
					throw ParseException.AsToken(token, Error.Analysis.UnknownGenericName);
				}

				//':'
				token = block.GetToken(index++, Error.Analysis.UnexpectedToken);
				if (token.Type != TokenType.Colon) {
					throw ParseException.AsToken(token, Error.Analysis.ColonExpected);
				}

				//Type(s)
				Token keywordConstraintToken = null;
				while (true) {
					token = block.GetToken(index, Error.Analysis.IdentifierExpected);

					if (token.Type == TokenType.@class ||
						token.Type == TokenType.@struct ||
						token.Type == TokenType.@interface ||
						token.Type == TokenType.@enum ||
						token.Type == TokenType.@delegate ||
						(token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.cpp)
					{
						if (keywordConstraintToken != null) {
							throw ParseException.AsToken(token, Error.Analysis.DuplicatedGenericConstraint);
						}
						if (targetItem.NewConstraint != null) {
							CheckNewConstraint(token, targetItem.NewConstraint);
						}
						keywordConstraintToken = token;
						++index;

						if (token.Type == TokenType.@enum) {
							//enum(int) constraint
							targetItem.KeywordConstraint = GenericKeywordConstraintType.@enum;
							token = block.GetToken(index, eofErrorMessage);
							if (token.Type == TokenType.LeftParenthesis) {
								++index;
								token = block.GetToken(index++, Error.Analysis.RightParenthesisExpected);
								if (!ConstantTable.IntegerTypeAlias.Contains(token.Type)) {
									throw ParseException.AsToken(token, Error.Analysis.EnumValueIntOnly);
								}
								targetItem.EnumTypeConstraint = (EnumValueType)Enum.Parse(typeof(EnumValueType), token.Text);
								token = block.GetToken(index++, Error.Analysis.RightParenthesisExpected);
								if (token.Type != TokenType.RightParenthesis) {
									throw ParseException.AsToken(token, Error.Analysis.RightParenthesisExpected);
								}
							}
						}
						else {
							//Other keyword constraints
							targetItem.KeywordConstraint = (GenericKeywordConstraintType)Enum.Parse(typeof(GenericKeywordConstraintType), token.Text);
						}
					}
					else if (token.Type == TokenType.@new) {
						if (targetItem.NewConstraint != null) {
							throw ParseException.AsToken(token, Error.Analysis.DuplicatedGenericConstraint);
						}
						CheckNewConstraint(keywordConstraintToken, token);
						targetItem.NewConstraint = (TokenKeyword)token;
						++index;
						//TODO: new(XXXX)
					}
					else {
						targetItem.TypeConstraints.Add((ReferenceToManagedType)TypeReferenceReader.Read(block, ref index, TypeParsingRule.None));
					}

					//Next constraint
					token = block.GetToken(index, eofErrorMessage);
					if (token.Type == TokenType.Comma) {
						++index;
					}
					else {
						break;
					}
				}
			}
		}

		static void CheckNewConstraint(Token token, Token newConstraint)
		{
			if (token != null) {
				if (token.Type == TokenType.@struct || 
					token.Type == TokenType.@enum ||
					token.Type == TokenType.@delegate) {
					throw ParseException.AsToken(newConstraint, Error.Analysis.NewConstraintInvalid);
				}
			}
		}
	}
}
