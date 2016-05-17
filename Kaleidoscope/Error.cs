namespace Kaleidoscope
{
	public static class Error
	{
		public static class Tokenizer
		{
			public const string UnexpectedEOF = "unexpected end of file";
			public const string UnknownToken = "unrecognized token";
			public const string InvalidIntegralConstant = "invalid integral constant";
			public const string InvalidRealConstant = "invalid real constant";
			public const string InvalidDecimalConstant = "invalid decimal constant";
			public const string UnknownUnicodeCharacter = "unrecognized unicode character escape sequence";
			public const string UnknownEscapeSequence = "unrecognized escape sequence";
			public const string NewLineNotAllowedOnChar = "new line is not allowed when declaring char constants";
			public const string EmptyCharLiteral = "empty character literal";
			public const string BadCharConstant = "bad char constant";
			public const string NewLineNotAllowedOnString = "new line is not allowed when declaring non-verbatim string constants";
			public const string MultiLineCommentNotClosed = "'*/' expected";
			public const string InvalidPreprocessor = "invalid preprocessor directive";
			public const string PreprocessorLineNotSupported = "'line' preprocessor not supported";
		}

		public static class Analysis
		{
			public const string IdentifierExpected = "identifier expected";
			public const string UnexpectedToken = "unexpected token";

			public const string SemicolonExpected = "; expected";
			public const string LeftBraceExpected = "{ expected";
			public const string RightBraceExpected = "} expected";
			public const string LeftParenthesisExpected = "( expected";
			public const string RightParenthesisExpected = ") expected";
			public const string LeftBracketExpected = "] expected";
			public const string RightBracketExpected = "] expected";
			public const string LeftArrowExpected = "< expected";
			public const string RightArrowExpected = "> expected";

			public const string VoidNotAllowed = "'void' is not allowed here";
			public const string VarNotAllowed = "'var' is not allowed here";
			public const string CppTypeNotAllowed = "C++ type is not allowed here";
			public const string MissingCppKeyword = "missing 'cpp::' keyword";

			public const string InvalidAttributeUsage = "invalid attribute usage";
			public const string InlineNotAllowed = "'inline' not allowed here";
			public const string DuplicatedModifier = "duplicated modifier";
			public const string InconsistentModifierOrder = "inconsistent modifier declare order";
			public const string ConflictModifier = "conflicted modifier";
			public const string InvalidModifier = "invalid modifier";
			public const string SealedOnlyWithOverride = "'sealed' can only be applied to 'override' member";
			public const string PartialWithClassOnly = "'partial' can only be applied to 'class', 'struct' and 'interface'";
			public const string ExternImpliesStatic = "'extern' implies 'static'";

			public const string DuplicatedGenericName = "duplicated generic type name";
			public const string UnknownGenericName = "unknown generic type name";
			public const string ColonExpected = ": expected";
			public const string DuplicatedGenericConstraint = "duplicated generic constraint";
			public const string NewConstraintInvalid = "'new' constraint is only allowed with 'class', 'interface' or 'cpp'";
			public const string EnumValueIntOnly = "only integer type (such as int or byte) are allowed as enum value type";

			public const string DuplicatedStaticConstructor = "duplicated static constructor";
			public const string StructNoDestructor = "value type cannot have destructor";
			public const string StaticConstructorNoParams = "static constructor cannot have parameters";
			public const string StructConstructorInvalid = "struct cannot contain explicit parameterless constructor";
			public const string DestructorNameInvalid = "name of destructor must match the name of the class";

			public const string ParameterThisFirstOnly = "modifier 'this' should be on the first parameter";
			public const string ParameterThisManagedOnly = "modifier 'this' can only apply to managed types";
			public const string ParameterThisStaticOnly = "extension method can only be declared in non-generic, non-nested static class";
			public const string ParamsMustBeLast = "a 'params' parameter must be the last parameter in a formal parameter list";
			public const string ParamsMustBeManagedType = "parameter with 'params' modifier cannot be a C++ type";
			public const string ParamsMustBeArray = "parameter with 'params' modifier must have an array type";
			public const string ParameterNoDefault = "a 'ref', 'out' or 'params' parameter cannot have default value";

			public const string MethodBodyExpected = "method body block expected";

		}
	}
}
