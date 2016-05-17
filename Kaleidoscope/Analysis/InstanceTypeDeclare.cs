﻿using System.Collections.Immutable;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public abstract class InstanceTypeDeclare : ManagedDeclare
	{
		public ImmutableArray<AttributeObject> CustomAttributes { get; protected set; }
		public abstract string Fullname { get; }

		protected InstanceTypeDeclare(TokenIdentifier name)
			: base(name)
		{
		}
	}
}
