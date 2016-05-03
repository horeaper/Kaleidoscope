using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Tokenizer
{
	public enum TokenType
	{
		//========================================================================
		// Symbols
		//========================================================================

		Dot,			// .
		Ampersand,		// &
		Comma,			// ,
		Colon,			// :
		DoubleColon,	// ::
		Semicolon,		// ;
		Question,		// ?
		PointerArrow,	// ->
		LeftArrow,		// <
		RightArrow,		// >
		Lambda,			// =>
		Asterisk,		// *

		NullConalescing,	// ??
		NullConditional,	// ?.

		LeftBrace,			// {
		RightBrace,			// }
		LeftParenthesis,	// (
		RightParenthesis,	// )
		LeftBracket,		// [
		RightBracket,		// ]

		Assign,		// =
		Add,		// +
		Minus,		// -
		Divide,		// /
		Mod,		// %
		Increment,	// ++
		Decrement,	// --
		AddAssign,	// +=
		MinAssign,	// -=
		MulAssign,	// *=
		DivAssign,	// /=
		ModAssign,	// %=

		GreatEqual,	// >=
		LessEqual,	// <=
		Equal,		// ==
		NotEqual,	// !=

		LogicalAnd,	// &&
		LogicalOr,	// ||
		LogicalNot,	// !

		BitwiseOr,  // |
		BitwiseNot, // ~
		BitwiseXor,	// ^
		ShiftLeft,	// <<
		ShiftRight,	// >>

		PipeLeft,	// <|
		PipeRight,	// |>

		//========================================================================
		// Others
		//========================================================================

		Keyword,
		Identifier,
		NumberLiteral,
		BooleanLiteral,
		Comment,
		Character,
		String,
		Preprocessor,
		Trivia,
	}
}
