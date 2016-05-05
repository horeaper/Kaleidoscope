using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

			var alias = ImmutableSortedSet.CreateBuilder<TokenType>();
			alias.Add(TokenType.@bool);
			alias.Add(TokenType.@sbyte);
			alias.Add(TokenType.@byte);
			alias.Add(TokenType.@short);
			alias.Add(TokenType.@ushort);
			alias.Add(TokenType.@int);
			alias.Add(TokenType.@uint);
			alias.Add(TokenType.@long);
			alias.Add(TokenType.@ulong);
			alias.Add(TokenType.@float);
			alias.Add(TokenType.@double);
			alias.Add(TokenType.@char);
			alias.Add(TokenType.@decimal);
			alias.Add(TokenType.@object);
			alias.Add(TokenType.@string);
			Alias = alias.ToImmutable();
		}

		public static readonly ImmutableDictionary<string, SymbolType> SymbolMap;
		public static readonly ImmutableSortedSet<TokenType> Alias;
	}
}
