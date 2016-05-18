using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class ParameterObject : ManagedDeclare
	{
		public readonly ImmutableArray<AttributeObject> CustomAttributes;
		public readonly ParameterKind ParameterKind;
		public readonly ReferenceToType Type;
		public readonly TokenBlock DefaultValueContent;
		public string Text { get; }

		public ParameterObject(TokenIdentifier name, IEnumerable<AttributeObject.Builder> customAttributes, ParameterKind parameterKind, ReferenceToType type, TokenBlock defaultValueContent)
			: base(name)
		{
			CustomAttributes = ImmutableArray.CreateRange(customAttributes.Select(item => new AttributeObject(item, this)));
			ParameterKind = parameterKind;
			Type = type;
			DefaultValueContent = defaultValueContent;

			var builder = new StringBuilder();
			if (ParameterKind != ParameterKind.None) {
				builder.Append(ParameterKind);
				builder.Append(' ');
			}
			builder.Append(Type.Text);
			if (DefaultValueContent != null) {
				builder.Append(" = ");
				builder.Append(DefaultValueContent.Text);
			}
			Text = builder.ToString();
		}
	}
}
