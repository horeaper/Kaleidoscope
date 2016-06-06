using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope
{
	public static class ConstantTable
	{
		static ConstantTable()
		{
			var symbolMap = ImmutableDictionary.CreateBuilder<string, SymbolType>();
			symbolMap.Add(".", SymbolType.Dot);
			symbolMap.Add("&", SymbolType.Ampersand);
			symbolMap.Add(",", SymbolType.Comma);
			symbolMap.Add(":", SymbolType.Colon);
			symbolMap.Add("::", SymbolType.DoubleColon);
			symbolMap.Add(";", SymbolType.Semicolon);
			symbolMap.Add("?", SymbolType.Question);
			symbolMap.Add("->", SymbolType.PointerArrow);
			symbolMap.Add("<", SymbolType.LeftArrow);
			symbolMap.Add(">", SymbolType.RightArrow);
			symbolMap.Add("=>", SymbolType.Lambda);
			symbolMap.Add("*", SymbolType.Asterisk);
			symbolMap.Add("??", SymbolType.NullConalescing);
			symbolMap.Add("?.", SymbolType.NullConditional);
			symbolMap.Add("{", SymbolType.LeftBrace);
			symbolMap.Add("}", SymbolType.RightBrace);
			symbolMap.Add("(", SymbolType.LeftParenthesis);
			symbolMap.Add(")", SymbolType.RightParenthesis);
			symbolMap.Add("[", SymbolType.LeftBracket);
			symbolMap.Add("]", SymbolType.RightBracket);
			symbolMap.Add("=", SymbolType.Assign);
			symbolMap.Add("+", SymbolType.Add);
			symbolMap.Add("-", SymbolType.Minus);
			symbolMap.Add("/", SymbolType.Divide);
			symbolMap.Add("%", SymbolType.Mod);
			symbolMap.Add("++", SymbolType.Increment);
			symbolMap.Add("--", SymbolType.Decrement);
			symbolMap.Add("+=", SymbolType.AddAssign);
			symbolMap.Add("-=", SymbolType.MinAssign);
			symbolMap.Add("*=", SymbolType.MulAssign);
			symbolMap.Add("/=", SymbolType.DivAssign);
			symbolMap.Add("%=", SymbolType.ModAssign);
			symbolMap.Add(">=", SymbolType.GreatEqual);
			symbolMap.Add("<=", SymbolType.LessEqual);
			symbolMap.Add("==", SymbolType.Equal);
			symbolMap.Add("!=", SymbolType.NotEqual);
			symbolMap.Add("&&", SymbolType.LogicalAnd);
			symbolMap.Add("||", SymbolType.LogicalOr);
			symbolMap.Add("!", SymbolType.LogicalNot);
			symbolMap.Add("|", SymbolType.BitwiseOr);
			symbolMap.Add("~", SymbolType.BitwiseNot);
			symbolMap.Add("^", SymbolType.BitwiseXor);
			symbolMap.Add("<<", SymbolType.ShiftLeft);
			symbolMap.Add(">>", SymbolType.ShiftRight);
			symbolMap.Add("<|", SymbolType.PipeLeft);
			symbolMap.Add("|>", SymbolType.PipeRight);
			SymbolMap = symbolMap.ToImmutable();

			Alias = ImmutableSortedSet.Create(
				TokenType.@bool,
				TokenType.@sbyte,
				TokenType.@byte,
				TokenType.@short,
				TokenType.@ushort,
				TokenType.@int,
				TokenType.@uint,
				TokenType.@long,
				TokenType.@ulong,
				TokenType.@float,
				TokenType.@double,
				TokenType.@char,
				TokenType.@decimal,
				TokenType.@object,
				TokenType.@string);

			IntegerTypeAlias = ImmutableSortedSet.Create(
				TokenType.@byte,
				TokenType.@sbyte,
				TokenType.@short,
				TokenType.@ushort,
				TokenType.@int,
				TokenType.@uint,
				TokenType.@long,
				TokenType.@ulong);

			AccessModifiers = ImmutableSortedSet.Create(
				TokenType.@public,
				TokenType.@protected,
				TokenType.@private,
				TokenType.@internal);

			InstanceKindModifier = ImmutableSortedSet.Create(
				TokenType.@virtual,
				TokenType.@override,
				TokenType.@abstract,
				TokenType.@static,
				TokenType.@extern,
				TokenType.@const);

			ValidNewInstanceKindModifier = ImmutableSortedSet.Create(
				TokenType.@virtual,
				TokenType.@abstract,
				TokenType.@static,
				TokenType.@extern);

			ValidParameterKindModifier = ImmutableSortedSet.Create(
				TokenType.@this,
				TokenType.@ref,
				TokenType.@out,
				TokenType.@params);

			ValidArithmeticOperators = ImmutableList.Create(
				new Tuple<TokenType, int>(TokenType.Add, 1),
				new Tuple<TokenType, int>(TokenType.Minus, 1),
				new Tuple<TokenType, int>(TokenType.LogicalNot, 1),
				new Tuple<TokenType, int>(TokenType.BitwiseNot, 1),
				new Tuple<TokenType, int>(TokenType.Increment, 1),
				new Tuple<TokenType, int>(TokenType.Decrement, 1),

				new Tuple<TokenType, int>(TokenType.Add, 2),
				new Tuple<TokenType, int>(TokenType.Minus, 2),
				new Tuple<TokenType, int>(TokenType.Asterisk, 2),
				new Tuple<TokenType, int>(TokenType.Divide, 2),
				new Tuple<TokenType, int>(TokenType.Mod, 2),
				new Tuple<TokenType, int>(TokenType.Ampersand, 2),
				new Tuple<TokenType, int>(TokenType.BitwiseOr, 2),
				new Tuple<TokenType, int>(TokenType.BitwiseXor, 2),
				new Tuple<TokenType, int>(TokenType.ShiftLeft, 2),
				new Tuple<TokenType, int>(TokenType.ShiftRight, 2),

				new Tuple<TokenType, int>(TokenType.Equal, 2),
				new Tuple<TokenType, int>(TokenType.NotEqual, 2),
				new Tuple<TokenType, int>(TokenType.LeftArrow, 2),
				new Tuple<TokenType, int>(TokenType.RightArrow, 2),
				new Tuple<TokenType, int>(TokenType.LessEqual, 2),
				new Tuple<TokenType, int>(TokenType.GreatEqual, 2));

			var validPropertyMethodAccessors = ImmutableDictionary.CreateBuilder<AccessModifier, AccessModifier[]>();
			validPropertyMethodAccessors.Add(AccessModifier.@public, new[] { AccessModifier.@protected, AccessModifier.@internal, AccessModifier.@private });
			validPropertyMethodAccessors.Add(AccessModifier.@protected, new[] { AccessModifier.@internal, AccessModifier.@private });
			validPropertyMethodAccessors.Add(AccessModifier.@private, new[] { AccessModifier.@internal });
			validPropertyMethodAccessors.Add(AccessModifier.@internal, new[] { AccessModifier.@private });
			ValidPropertyMethodAccessors = validPropertyMethodAccessors.ToImmutable();
		}

		public static readonly ImmutableDictionary<string, SymbolType> SymbolMap;
		public static readonly ImmutableSortedSet<TokenType> Alias;
		public static readonly ImmutableSortedSet<TokenType> IntegerTypeAlias;
		public static readonly ImmutableSortedSet<TokenType> AccessModifiers;
		public static readonly ImmutableSortedSet<TokenType> InstanceKindModifier;
		public static readonly ImmutableSortedSet<TokenType> ValidNewInstanceKindModifier;
		public static readonly ImmutableSortedSet<TokenType> ValidParameterKindModifier;
		public static readonly ImmutableList<Tuple<TokenType, int>> ValidArithmeticOperators;
		public static readonly ImmutableDictionary<AccessModifier, AccessModifier[]> ValidPropertyMethodAccessors;
	}
}
