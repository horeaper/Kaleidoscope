using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public abstract class NestedInstanceTypeDeclare : UserTypeDeclare
	{
		public readonly AccessModifier AccessModifier;
		public readonly bool IsNew;

		protected NestedInstanceTypeDeclare(Builder builder)
		{
			AccessModifier = builder.AccessModifier;
			IsNew = builder.IsNew;
		}

		public abstract class Builder
		{
			public AccessModifier AccessModifier;
			public bool IsNew;
		}
	}
}
