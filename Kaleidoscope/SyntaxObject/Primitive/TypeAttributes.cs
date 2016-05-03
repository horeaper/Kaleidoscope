using System;

namespace Kaleidoscope.SyntaxObject.Primitive
{
	[Flags]
	public enum TypeAttributes
	{
		Public      = 1 << 0,
		Protected   = 1 << 1,
		Private     = 1 << 2,
		Internal    = 1 << 3,

		Class       = 1 << 4,	// Type is a class (reference type)
		Struct      = 1 << 5,	// Type is a struct (value type)
		Interface   = 1 << 6,	// Type is a interface (special reference type)
		Enum        = 1 << 7,	// Type is a enum (special value type)
		Delegate    = 1 << 8,	// Type is a delegate (reference type)

		Abstract    = 1 << 9,	// The class is abstract and must be overridden
		Sealed      = 1 << 10,	// The class is concrete and cannot be extended
		Static      = 1 << 11,	// The class is static and cannot contains instance member
		Unsafe      = 1 << 12,	// The class is marked as unsafe

		Array       = 1 << 13,	// Type is an array
		GenericType = 1 << 14,	// Type is a generic type
		Nested      = 1 << 15,	// Current Type object represents a type whose definition is nested inside the definition of another type
		Primitive	= 1 << 16,	// Type is one of the primitive types (bool, byte, sbyte, short, ushort, int, uint, long, ulong, IntPtr, UIntPtr, char, float, and double)
	}
}
