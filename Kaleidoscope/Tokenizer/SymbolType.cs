namespace Kaleidoscope.Tokenizer
{
	public enum SymbolType
	{
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
	}
}
