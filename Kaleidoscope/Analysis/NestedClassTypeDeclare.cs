using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class NestedClassTypeDeclare : ClassTypeDeclare
	{
		public readonly AccessModifier AccessModifier;
		public readonly ClassTypeDeclare ContainerType;
		public override string Fullname { get; }

		public NestedClassTypeDeclare(Builder builder)
			: base(builder)
		{
			AccessModifier = builder.AccessModifier;
			ContainerType = builder.ContainerType;
			Fullname = ContainerType.Fullname + "." + Name.Text;
		}

		public new sealed class Builder : ClassTypeDeclare.Builder
		{
			public AccessModifier AccessModifier;
			public ClassTypeDeclare ContainerType;
		}
	}
}
