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
			UsingStaticDirectives = ImmutableArray.CreateRange(builder.UsingStaticDirectives);
			UsingCSNamespaceDirectives = ImmutableArray.CreateRange(builder.UsingCSNamespaceDirectives);
			UsingCSAliasDirectives = ImmutableArray.CreateRange(builder.UsingCSAliasDirectives);
			UsingCppNamespaceDirectives = ImmutableArray.CreateRange(builder.UsingCppNamespaceDirectives);
			UsingCppAliasDirectives = ImmutableArray.CreateRange(builder.UsingCppAliasDirectives);
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
