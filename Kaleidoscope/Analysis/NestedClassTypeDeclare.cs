using System;
using System.Collections.Immutable;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class NestedClassTypeDeclare : ClassTypeDeclare
	{
		public ClassTypeDeclare ContainerType { get; }
		public AccessModifier AccessModifier { get; }
		public bool IsNew { get; }
		public override string Fullname { get; }

		public NestedClassTypeDeclare(TokenIdentifier name, AttributeObject[] customAttributes, Func<NestedClassTypeDeclare, Builder> fnReadNember)
			: base(name, customAttributes)
		{
			var builder = fnReadNember(this);
			ContainerType = builder.ContainerType;
			AccessModifier = builder.AccessModifier;
			IsNew = builder.IsNew;
			ApplyMembers(builder);

			Fullname = ContainerType.Fullname + "." + Name.Text;
		}

		public new sealed class Builder : ClassTypeDeclare.Builder
		{
			public ClassTypeDeclare ContainerType;
			public AccessModifier AccessModifier;
			public bool IsNew;
		}
	}
}
