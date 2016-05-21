using System;
using System.Collections.Generic;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	static class ParameterReader
	{
		public static List<ParameterObject> Read(InfoOutput infoOutput, TokenBlock block, bool isAllowThisParameter)
		{
			if (block.Count == 0) {
				return new List<ParameterObject>();
			}


			var result = new List<ParameterObject>();
			var currentAttributes = new List<AttributeObject.Builder>();
			TokenKeyword parameterKindModifier = null;

			int index = 0;
			while (true) {
				var token = block.GetToken(index, Error.Analysis.IdentifierExpected);
				if (token.Type == TokenType.LeftBracket) {
					++index;
					currentAttributes.Add(AttributeObjectReader.Read(block, ref index));
				}
				else if (ConstantTable.ValidParameterKindModifier.Contains(token.Type)) {
					if (parameterKindModifier != null) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.ConflictModifier));
					}
					if (token.Type == TokenType.@this) {
						if (result.Count > 0) {
							infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.ParameterThisFirstOnly));
						}
						if (!isAllowThisParameter) {
							infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.ParameterThisStaticOnly));
						}
					}
					parameterKindModifier = (TokenKeyword)token;
					++index;
				}
				else {
					var kind = ParameterKind.None;
					if (parameterKindModifier != null) {
						kind = (ParameterKind)Enum.Parse(typeof(ParameterKind), parameterKindModifier.Type.ToString());
					}

					var returnType = TypeReferenceReader.Read(block, ref index, TypeParsingRule.AllowCppType | TypeParsingRule.AllowArray);
					var managedReturnType = returnType as ReferenceToManagedType;
					if (kind ==	ParameterKind.@this && managedReturnType == null) {
						infoOutput.OutputError(ParseException.AsTokenBlock(returnType.Content, Error.Analysis.ParameterThisManagedOnly));
					}
					else if (kind == ParameterKind.@params) {
						if (managedReturnType == null) {
							infoOutput.OutputError(ParseException.AsTokenBlock(returnType.Content, Error.Analysis.ParamsMustBeManagedType));
						}
						else if (managedReturnType.ArrayRank == 0) {
							infoOutput.OutputError(ParseException.AsTokenBlock(returnType.Content, Error.Analysis.ParamsMustBeArray));
						}
					}

					var nameToken = block.GetToken(index++, Error.Analysis.IdentifierExpected);
					if (nameToken.Type != TokenType.Identifier) {
						--index;
						infoOutput.OutputError(ParseException.AsToken(nameToken, Error.Analysis.IdentifierExpected));
					}

					token = block.GetToken(index);
					if (token == null) {
						result.Add(new ParameterObject((TokenIdentifier)nameToken, currentAttributes, kind, returnType, null));
						for (int cnt = 0; cnt < result.Count - 1; ++cnt) {
							if (result[cnt].ParameterKind == ParameterKind.@params) {
								infoOutput.OutputError(ParseException.AsTokenBlock(result[cnt].Type.Content, Error.Analysis.ParamsMustBeLast));
							}
						}
						return result;
					}
					else if (token.Type == TokenType.Comma) {
						result.Add(new ParameterObject((TokenIdentifier)nameToken, currentAttributes, kind, returnType, null));
						currentAttributes.Clear();
						parameterKindModifier = null;
						++index;
					}
					else if (token.Type == TokenType.Assign) {
						if (kind == ParameterKind.@ref || kind == ParameterKind.@out || kind == ParameterKind.@params) {
							infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.ParameterNoDefault));
						}

						++index;
						var content = block.ReadPastSpecificToken(ref index, TokenType.Comma);
						result.Add(new ParameterObject((TokenIdentifier)nameToken, currentAttributes, kind, returnType, content));
						currentAttributes.Clear();
						parameterKindModifier = null;
					}
					else {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.UnexpectedToken));
						++index;
					}
				}
			}
		}
	}
}
