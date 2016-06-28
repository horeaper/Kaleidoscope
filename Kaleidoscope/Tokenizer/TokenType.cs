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
		PipeRight,  // |>

		//========================================================================
		// Keywords
		//========================================================================

		@abstract,
		@as,
		@base,
		@bool,
		@break,
		@byte,
		@case,
		@catch,
		@char,
		@checked,
		@class,
		@const,
		@continue,
		@decimal,
		@default,
		@delegate,
		@do,
		@double,
		dynamic,
		@else,
		@enum,
		@event,
		@explicit,
		@extern,
		@false,
		@finally,
		@fixed,
		@float,
		@for,
		@foreach,
		@goto,
		@if,
		@implicit,
		@in,
		@int,
		@interface,
		@internal,
		@is,
		@lock,
		@long,
		@namespace,
		@nameof,
		@new,
		@null,
		@object,
		@operator,
		@out,
		@override,
		@params,
		@private,
		@protected,
		@public,
		@readonly,
		@ref,
		@return,
		@sbyte,
		@sealed,
		@short,
		@sizeof,
		@stackalloc,
		@static,
		@string,
		@struct,
		@switch,
		@this,
		@throw,
		@true,
		@try,
		@typeof,
		@uint,
		@ulong,
		@unchecked,
		@unsafe,
		@ushort,
		@using,
		var,
		@virtual,
		@void,
		@volatile,
		@while,

		//========================================================================
		// Trivia
		//========================================================================

		NewLine,
		Space,
		Tab,

		//========================================================================
		// Others
		//========================================================================

		Identifier,
		NumberLiteral,
		BooleanLiteral,
		Comment,
		Character,
		StringLiteral,
		Preprocessor,
	}
}
