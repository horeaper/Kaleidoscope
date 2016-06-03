using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.SyntaxObject
{
	public class TokenItem<T1, T2> where T2 : Token
	{
		public readonly T1 Item;
		public readonly T2 Token;

		public TokenItem(T1 item, T2 token)
		{
			Item = item;
			Token = token;
		}
	}

#region Member modifiers

	public class ClassTypeKindItem : TokenItem<ClassTypeKind, TokenKeyword>
	{
		public ClassTypeKindItem(ClassTypeKind item, TokenKeyword token)
			: base(item, token)
		{
		}
	}

	public class AccessModifierItem : TokenItem<AccessModifier, TokenKeyword>
	{
		public AccessModifierItem(AccessModifier item, TokenKeyword token)
			: base(item, token)
		{
		}
	}

	public class TypeInstanceKindItem : TokenItem<TypeInstanceKind, TokenKeyword>
	{
		public TypeInstanceKindItem(TypeInstanceKind item, TokenKeyword token)
			: base(item, token)
		{
		}
	}

	public class MethodInstanceKindItem : TokenItem<MethodInstanceKind, TokenKeyword>
	{
		public MethodInstanceKindItem(MethodInstanceKind item, TokenKeyword token)
			: base(item, token)
		{
		}
	}

	public class ParameterKindItem : TokenItem<ParameterKind, TokenKeyword>
	{
		public ParameterKindItem(ParameterKind item, TokenKeyword token)
			: base(item, token)
		{
		}
	}

	public class IsPartialItem : TokenItem<bool, TokenIdentifier>
	{
		public IsPartialItem(bool item, TokenIdentifier token)
			: base(item, token)
		{
		}
	}

	public class IsSealedItem : TokenItem<bool, TokenIdentifier>
	{
		public IsSealedItem(bool item, TokenIdentifier token)
			: base(item, token)
		{
		}
	}

	public class IsAsyncItem : TokenItem<bool, TokenIdentifier>
	{
		public IsAsyncItem(bool item, TokenIdentifier token)
			: base(item, token)
		{
		}
	}

	public class IsReadonlyItem : TokenItem<bool, TokenKeyword>
	{
		public IsReadonlyItem(bool item, TokenKeyword token)
			: base(item, token)
		{
		}
	}

	public class IsConstItem : TokenItem<bool, TokenKeyword>
	{
		public IsConstItem(bool item, TokenKeyword token)
			: base(item, token)
		{
		}
	}

#endregion
}
