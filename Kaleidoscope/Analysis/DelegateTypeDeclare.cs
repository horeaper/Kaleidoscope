using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Kaleidoscope.Analysis
{
	public abstract class DelegateTypeDeclare : InstanceTypeDeclare
	{
		public readonly ReferenceToType ReturnType;
		public readonly ImmutableArray<GenericDeclare> GenericTypes;
		public readonly ImmutableArray<ParameterObject> Parameters;

		protected DelegateTypeDeclare(Builder builder)
			: base(builder)
		{
			ReturnType = builder.ReturnType;
			Parameters = ImmutableArray.CreateRange(builder.Parameters);
			GenericTypes = ImmutableArray.CreateRange(builder.GenericTypes.Select(item => new GenericDeclare(item)));
		}

		public new abstract class Builder : InstanceTypeDeclare.Builder
		{
			public ReferenceToType ReturnType;
			public IEnumerable<GenericDeclare.Builder> GenericTypes;
			public IEnumerable<ParameterObject> Parameters;
		}
	}
}
