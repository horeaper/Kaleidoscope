using System;

namespace Kaleidoscope.SyntaxObject.Primitive
{
	[Flags]
	public enum MethodAttributes
	{
		Public      = 1 << 0,	// This method is public
		Protocted   = 1 << 1,	// This method is protected	
		Private     = 1 << 2,	// This method is private
		Internal    = 1 << 3,	// This method is internal

		Static      = 1 << 4,	// Indicates that the method is defined on the type; otherwise, it is defined per instance
		Abstract    = 1 << 5,	// The class does not provide an implementation of this method
		Virtual     = 1 << 6,	// Indicates that the method is virtual
		Override    = 1 << 7,	// Indicates that the method is virtual and overrides parent method
		Sealed      = 1 << 8,   // Indicates that this method cannot be overridden
		New		    = 1 << 9,	// Indicates that this method override the declaration of the parent method
		Extern      = 1 << 10,	// The method is an extern method
		Unsafe      = 1 << 11,	// The method is marked as unsafe
		Async	    = 1 << 12,	// The method is marked as asynchronous

		Constructor = 1 << 13,	// The method is a constructor
		Destructor  = 1 << 14,	// The method is a constructor
		Generic     = 1 << 15,	// The method is generic
		Property    = 1 << 16,	// The method is a property setter/getter
	}
}
