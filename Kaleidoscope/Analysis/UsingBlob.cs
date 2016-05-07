using System.Collections.Generic;
using System.Collections.Immutable;

namespace Kaleidoscope.Analysis
{
	/// <summary>
	/// A group of usings
	/// </summary>
	public sealed class UsingBlob
	{
		public readonly ImmutableArray<UsingStaticDirective> UsingStaticDirectives;
		public readonly ImmutableArray<UsingCSNamespaceDirective> UsingCSNamespaceDirectives;
		public readonly ImmutableArray<UsingCSAliasDirective> UsingCSAliasDirectives;
		public readonly ImmutableArray<UsingCppNamespaceDirective> UsingCppNamespaceDirectives;
		public readonly ImmutableArray<UsingCppAliasDirective> UsingCppAliasDirectives;

		public UsingBlob(Builder builder)
		{
			UsingStaticDirectives = ImmutableArray.Create(builder.UsingStaticDirectives.ToArray());
			UsingCSNamespaceDirectives = ImmutableArray.Create(builder.UsingCSNamespaceDirectives.ToArray());
			UsingCSAliasDirectives = ImmutableArray.Create(builder.UsingCSAliasDirectives.ToArray());
			UsingCppNamespaceDirectives = ImmutableArray.Create(builder.UsingCppNamespaceDirectives.ToArray());
			UsingCppAliasDirectives = ImmutableArray.Create(builder.UsingCppAliasDirectives.ToArray());
		}

		public sealed class Builder
		{
			public readonly List<UsingStaticDirective> UsingStaticDirectives = new List<UsingStaticDirective>();
			public readonly List<UsingCSNamespaceDirective> UsingCSNamespaceDirectives = new List<UsingCSNamespaceDirective>();
			public readonly List<UsingCSAliasDirective> UsingCSAliasDirectives = new List<UsingCSAliasDirective>();
			public readonly List<UsingCppNamespaceDirective> UsingCppNamespaceDirectives = new List<UsingCppNamespaceDirective>();
			public readonly List<UsingCppAliasDirective> UsingCppAliasDirectives = new List<UsingCppAliasDirective>();

			public Builder Clone()
			{
				var right = new Builder();
				right.UsingStaticDirectives.AddRange(UsingStaticDirectives);
				right.UsingCSNamespaceDirectives.AddRange(UsingCSNamespaceDirectives);
				right.UsingCSAliasDirectives.AddRange(UsingCSAliasDirectives);
				right.UsingCppNamespaceDirectives.AddRange(UsingCppNamespaceDirectives);
				right.UsingCppAliasDirectives.AddRange(UsingCppAliasDirectives);
				return right;
			}
		}
	}
}
