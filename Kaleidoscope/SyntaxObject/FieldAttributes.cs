using System;

namespace Kaleidoscope.SyntaxObject
{
	[Flags]
	public enum FieldAttributes
	{
		Public     = 1 << 0,	// This field is public
		Protocted  = 1 << 1,	// This field is protected	
		Internal   = 1 << 2,	// This field is internal
		Private    = 1 << 3,	// This field is private

		Static     = 1 << 4,	// This field is per-instance
		Literal    = 1 << 5,	// This field's value is a compile-time (static or early bound) constant
		Readonly   = 1 << 6,	// This field can be set only in the body of the constructor
		New		   = 1 << 7,	// This field override the declaration of the parent field
	}
}
