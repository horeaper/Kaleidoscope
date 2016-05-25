using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class AnalysisCodeFileCS
	{
		T ReadInterfaceMembers<T>(ClassTraits classTraints) where T : ClassTypeDeclare.Builder, new()
		{
			var builder = new T {
				TypeKind = ClassTypeKind.@interface,
				CustomAttributes = classTraints.CustomAttributes,
				Name = classTraints.Name,
				InstanceKind = classTraints.InstanceKind,
				IsPartial = classTraints.IsPartial,
				GenericTypes = classTraints.GenericTypes,
				Inherits = classTraints.Inherits
			};
			var token = block.GetToken(index++, Error.Analysis.LeftBraceExpected);
			if (token.Type != TokenType.LeftBrace) {
				throw ParseException.AsToken(token, Error.Analysis.LeftBraceExpected);
			}

			while (true) {
				token = block.GetToken(index++, Error.Analysis.RightBraceExpected);

				if (token.Type == TokenType.RightBrace) {
					return builder;
				}
				else if (ConstantTable.AccessModifiers.Contains(token.Type) ||
						 ConstantTable.InstanceKindModifier.Contains(token.Type) ||
						 token.Type == TokenType.@readonly ||
						 token.Type == TokenType.@unsafe) {
					throw ParseException.AsToken(token, Error.Analysis.InvalidModifier);
				}

			}
		}
	}
}
