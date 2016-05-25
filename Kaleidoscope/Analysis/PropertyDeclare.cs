using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public abstract class PropertyDeclare : MemberDeclare
	{
		public readonly bool IsNew;
		public readonly bool IsSealed;
		public readonly PropertyInstanceKind InstanceKind; 
		public readonly ReferenceToType Type;
		public readonly ReferenceToType ExplicitInterface;
		public readonly PropertyMethodDeclare GetterMethod;
		public readonly PropertyMethodDeclare SetterMethod;

		protected PropertyDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			IsNew = builder.IsNew;
			IsSealed = builder.IsSealed;
			InstanceKind = builder.InstanceKind;
			Type = builder.Type;
			ExplicitInterface = builder.ExplicitInterface;
			GetterMethod = new PropertyMethodDeclare(builder.GetterMethod, owner);
			SetterMethod = builder.SetterMethod != null ? new PropertyMethodDeclare(builder.SetterMethod, owner) : null;
		}

		protected void PrintInstanceKind(StringBuilder builder)
		{
			if (IsNew) {
				builder.Append("new ");
			}
			if (IsSealed) {
				builder.Append("sealed ");
			}
			if (InstanceKind != PropertyInstanceKind.None) {
				builder.Append(InstanceKind);
				builder.Append(' ');
			}
		}

		protected void PrintMethods(StringBuilder builder)
		{
			builder.Append(" { get; ");
			if (SetterMethod != null) {
				builder.Append("set; ");
			}
			builder.Append('}');
		}

		public new abstract class Builder : MemberDeclare.Builder
		{
			public bool IsNew;
			public bool IsSealed;
			public PropertyInstanceKind InstanceKind;
			public ReferenceToType Type;
			public ReferenceToType ExplicitInterface;
			public PropertyMethodDeclare.Builder GetterMethod;
			public PropertyMethodDeclare.Builder SetterMethod;
		}
	}
}
