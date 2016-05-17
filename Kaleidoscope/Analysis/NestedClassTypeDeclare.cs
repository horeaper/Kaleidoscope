using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class NestedClassTypeDeclare : ClassTypeDeclare
	{
		public readonly ClassTypeDeclare ContainerType;
		public readonly AccessModifier AccessModifier;
		public readonly bool IsNew;
		public override string Fullname { get; }

		public NestedClassTypeDeclare(Builder builder, ClassTypeDeclare containerType)
			: base(builder)
		{
			ContainerType = containerType;
			AccessModifier = builder.AccessModifier;
			IsNew = builder.IsNew;

			Fullname = ContainerType.Fullname + "." + Name.Text;
		}

		public new sealed class Builder : ClassTypeDeclare.Builder
		{
			public AccessModifier AccessModifier;
			public bool IsNew;
		}
	}
}
