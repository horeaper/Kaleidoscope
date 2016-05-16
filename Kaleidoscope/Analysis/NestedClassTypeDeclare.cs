using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class NestedClassTypeDeclare : ClassTypeDeclare
	{
		public readonly AccessModifier AccessModifier;
		public readonly bool IsNew;
		public readonly ClassTypeDeclare ContainerType;
		public override string Fullname { get; }

		public NestedClassTypeDeclare(Builder builder)
			: base(builder)
		{
			AccessModifier = builder.AccessModifier;
			IsNew = builder.IsNew;
			ContainerType = builder.ContainerType;
			Fullname = ContainerType.Fullname + "." + Name.Text;
		}

		public new sealed class Builder : ClassTypeDeclare.Builder
		{
			public AccessModifier AccessModifier;
			public bool IsNew;
			public ClassTypeDeclare ContainerType;
		}
	}
}
