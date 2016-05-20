using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Kaleidoscope.Analysis
{
	public sealed class IndexerDeclare : PropertyDeclare
	{
		public readonly ImmutableArray<ParameterObject> Parameters;
		readonly string m_displayName;

		public IndexerDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			Parameters = ImmutableArray.CreateRange(builder.Parameters);

			var text = new StringBuilder();
			text.Append("[Indexer] ");
			PrintAccessModifier(text);
			PrintInstanceKind(text);
			text.Append(Type.Text);
			text.Append(' ');
			if (ExplicitInterface != null) {
				text.Append(ExplicitInterface.Text);
				text.Append('.');
			}
			text.Append(Name.Text);
			text.Append('[');
			for (int cnt = 0; cnt < Parameters.Length; ++cnt) {
				text.Append(Parameters[cnt].Text);
				if (cnt < Parameters.Length - 1) {
					text.Append(", ");
				}
			}
			text.Append(']');
			m_displayName = text.ToString();
		}

		public override string ToString()
		{
			return m_displayName;
		}

		public new sealed class Builder : PropertyDeclare.Builder
		{
			public IEnumerable<ParameterObject> Parameters;
		}
	}
}
