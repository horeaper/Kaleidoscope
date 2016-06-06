using System;
using System.Collections.Generic;
using System.Linq;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class AnalysisCodeFileCS
	{
		EnumTypeDeclare ReadEnumMembers(TypeTraits traits)
		{
			foreach (var item in traits.GenericTypes) {
				infoOutput.OutputError(ParseException.AsToken(item.Name, Error.Analysis.EnumGenericNotAllowed));
			}
			var builder = new EnumTypeDeclare.Builder {
				Name = traits.Name,
				CustomAttributes = traits.CustomAttributes,
			};

			//enum value type
			if (traits.Inherits.Count > 0) {
				for (int cnt = 1; cnt < traits.Inherits.Count; ++cnt) {
					infoOutput.OutputError(ParseException.AsTokenBlock(traits.Inherits[cnt].Content, Error.Analysis.EnumTypeInvalid));
				}
				if (traits.Inherits[0].Content.Count > 1) {
					infoOutput.OutputError(ParseException.AsTokenBlock(traits.Inherits[0].Content, Error.Analysis.EnumTypeInvalid));
				}
				else {
					if (!Enum.TryParse(traits.Inherits[0].Content.First.Text, out builder.ValueType)) {
						builder.ValueType = EnumValueType.@int;
						infoOutput.OutputError(ParseException.AsTokenBlock(traits.Inherits[0].Content, Error.Analysis.EnumTypeInvalid));
					}
				}
			}

			var token = block.GetToken(index++, Error.Analysis.LeftBraceExpected);
			if (token.Type != TokenType.LeftBrace) {
				throw ParseException.AsToken(token, Error.Analysis.LeftBraceExpected);
			}

			var currentAttributes = new List<AttributeObject.Builder>();
			while (true) {
				token = block.GetToken(index++, Error.Analysis.RightBraceExpected);

				if (token.Type == TokenType.RightBrace) {
					return new EnumTypeDeclare(builder);
				}
				else if (token.Type == TokenType.LeftBracket) {
					currentAttributes.Add(AttributeObjectReader.Read(block, ref index));
				}
				else if (token.Type == TokenType.Identifier) {
					var item = new EnumItemObject.Builder {
						Name = (TokenIdentifier)token,
						CustomAttributes = currentAttributes.ToArray(),
					};
					currentAttributes.Clear();

					//Check conflict
					bool isConflict = builder.Items.Any(existingItem => existingItem.Name.Text == item.Name.Text);
					if (isConflict) {
						infoOutput.OutputError(ParseException.AsToken(item.Name, Error.Analysis.DuplicateEnumMember));
					}

					//Default value
					token = block.GetToken(index, Error.Analysis.RightBraceExpected);
					if (token.Type == TokenType.Assign) {
						++index;
						item.DefaultValueContent = block.ReadPastSpecificTokens(ref index, TokenType.Comma, TokenType.RightBrace);
						--index;
						token = block.GetToken(index, Error.Analysis.RightBraceExpected);
					}

					//Next: ',' or '}'
					if (token.Type == TokenType.Comma) {
						if (!isConflict) {
							builder.Items.Add(item);
						}
						++index;
					}
					else if (token.Type == TokenType.RightBrace) {
						if (!isConflict) {
							builder.Items.Add(item);
						}
					}
					else {
						throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
					}
				}
				else {
					throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
				}
			}
		}
	}
}
