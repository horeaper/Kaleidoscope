using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public ParameterObject(TokenIdentifier name, IEnumerable<AttributeObject.Builder> customAttributes, ParameterKind parameterKind, ReferenceToType type, TokenBlock defaultValueContent)
			: base(name)
		{
			CustomAttributes = ImmutableArray.CreateRange(customAttributes.Select(item => new AttributeObject(item, this)));
			ParameterKind = parameterKind;
			Type = type;
			DefaultValueContent = defaultValueContent;
		}
	}
}
