using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		NullConalescing,	// ??
		NullConditional,	// ?.

		LeftBrace,			// {
		RightBrace,			// }
		LeftParenthesis,	// (
		RightParenthesis,	// )
		LeftBracket,		// [
		RightBracket,		// ]

		OperatorAssign,		// =
		OperatorAdd,		// +
		OperatorMin,		// -
		OperatorMul,		// *
		OperatorDiv,		// /
		OperatorMod,		// %
		OperatorSelfAdd,	// ++
		OperatorSelfMin,	// --
		OperatorAddAssign,	// +=
		OperatorMinAssign,	// -=
		OperatorMulAssign,	// *=
		OperatorDivAssign,	// /=
		OperatorModAssign,	// %=

		OperatorGreatEqual,	// >=
		OperatorLessEqual,	// <=
		OperatorEqual,		// ==
		OperatorNotEqual,	// !=

		OperatorLogicAnd,	// &&
		OperatorLogicOr,	// ||
		OperatorLogicNot,	// !

		OperatorBitwiseOr,	// |
		OperatorBitwiseXor,	// ^
		OperatorBitwiseNot,	// ~
		OperatorShiftLeft,	// <<
		OperatorShiftRight,	// >>
	}
}
