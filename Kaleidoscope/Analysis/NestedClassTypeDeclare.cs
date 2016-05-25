﻿using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class NestedClassTypeDeclare : ClassTypeDeclare
	{
		public readonly ClassTypeDeclare ContainerType;
		public readonly AccessModifier AccessModifier;
		public readonly bool IsNew;
		public override string Fullname { get; }
		readonly string m_displayName;

		public NestedClassTypeDeclare(Builder builder, ClassTypeDeclare containerType)
			: base(builder)
		{
			ContainerType = containerType;
			AccessModifier = builder.AccessModifier;
			IsNew = builder.IsNew;

			Fullname = ContainerType.Fullname + "." + Name.Text;

			var text = new StringBuilder();
			text.Append("[NestedClassTypeDeclare] ");
			if (AccessModifier != AccessModifier.@private) {
				text.Append(AccessModifier);
				text.Append(' ');
			}
			if (IsNew) {
				text.Append("new ");
			}
			text.Append(TypeKind);
			text.Append(' ');
			text.Append(Fullname);
			m_displayName = text.ToString();
		}

		public override string ToString()
		{
			return m_displayName;
		}

		public new sealed class Builder : ClassTypeDeclare.Builder
		{
			public AccessModifier AccessModifier;
			public bool IsNew;
		}
	}
}
