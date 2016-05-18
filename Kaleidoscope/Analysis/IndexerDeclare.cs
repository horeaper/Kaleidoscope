using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			if (IsNew) {
				text.Append("new ");
			}
			PrintInstanceKind(text);
			text.Append(Type.Text);
			text.Append(' ');
			text.Append(NameContent.Text);
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
