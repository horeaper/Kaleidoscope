using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class DelegateTypeDeclare : InstanceTypeDeclare
	{
		public readonly ReferenceToType ReturnType;
		public readonly ImmutableArray<GenericDeclare> GenericTypes;
		public readonly ImmutableArray<ParameterObject> Parameters;

		public override string DisplayName { get; }
		public override IEnumerable<GenericDeclare> DeclaredGenerics => GenericTypes;

		public DelegateTypeDeclare(Builder builder)
			: base(builder)
		{
			ReturnType = builder.ReturnType;
			Parameters = ImmutableArray.CreateRange(builder.Parameters);
			GenericTypes = ImmutableArray.CreateRange(builder.GenericTypes.Select(item => new GenericDeclare(item)));

			var text = new StringBuilder();
			text.Append("delegate ");
			text.Append(ReturnType.Text);
			text.Append(' ');
			text.Append(Name.Text);
			if (GenericTypes.Length > 0) {
				text.Append('<');
				for (int cnt = 0; cnt < GenericTypes.Length; ++cnt) {
					text.Append(GenericTypes[cnt].Text);
					if (cnt < GenericTypes.Length - 1) {
						text.Append(", ");
					}
				}
				text.Append('>');
			}
			text.Append('(');
			for (int cnt = 0; cnt < Parameters.Length; ++cnt) {
				text.Append(Parameters[cnt].Text);
				if (cnt < Parameters.Length - 1) {
					text.Append(", ");
				}
			}
			text.Append(')');
			DisplayName = text.ToString();
		}

		public new sealed class Builder : InstanceTypeDeclare.Builder
		{
			public ReferenceToType ReturnType;
			public IEnumerable<GenericDeclare.Builder> GenericTypes;
			public IEnumerable<ParameterObject> Parameters;
		}
	}
}
