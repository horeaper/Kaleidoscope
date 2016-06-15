using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class DeclaredNamespaceOrTypeName
	{
		public readonly DeclaredNamespaceOrTypeName Owner;
		public readonly NamespaceOrTypeName Name;
		public readonly bool IsNamespace;
		public readonly ImmutableArray<DeclaredNamespaceOrTypeName> NamespaceOrTypeName;
		public readonly ImmutableArray<DeclaredManagedType> Types;
		public string Fullname { get; }

		public DeclaredNamespaceOrTypeName(Builder builder, DeclaredNamespaceOrTypeName owner)
		{
			Owner = owner;
			Name = new NamespaceOrTypeName(builder.Name);
			IsNamespace = builder.IsNamespace;
			NamespaceOrTypeName = ImmutableArray.CreateRange(builder.NamespaceOrTypeNames.Select(item => new DeclaredNamespaceOrTypeName(item, this)));
			Types = ImmutableArray.CreateRange(builder.Types.Select(item => new DeclaredManagedType(item, this)));

			if (owner != null) {
				var text = new StringBuilder();
				text.Append(Name.DisplayName);
				var currentNsOrTN = Owner;
				while (currentNsOrTN?.Name.Name != null) {
					text.Insert(0, '.');
					text.Insert(0, currentNsOrTN.Name.DisplayName);
					currentNsOrTN = currentNsOrTN.Owner;
				}
				Fullname = text.ToString();
			}
			else {
				Fullname = "(global)";
			}
		}

		public override string ToString()
		{
			if (IsNamespace) {
				return "[DeclaredNamespace] " + Fullname;
			}
			else {
				return "[DeclaredTypeName] " + Fullname;
			}
		}

		public sealed class Builder
		{
			public readonly NamespaceOrTypeName.Builder Name = new NamespaceOrTypeName.Builder();
			public bool IsNamespace;
			public readonly List<Builder> NamespaceOrTypeNames = new List<Builder>();
			public readonly List<DeclaredManagedType.Builder> Types = new List<DeclaredManagedType.Builder>();

			public Builder GetContainerByNamespace(ImmutableArray<TokenIdentifier> @namespace)
			{
				var currentBuilder = this;
				foreach (var ns in @namespace) {
					var target = currentBuilder.NamespaceOrTypeNames.FirstOrDefault(item => item.Name.Name == ns.Text && item.Name.Generics.Count == 0);
					if (target == null) {
						target = new Builder();
						target.Name.Name = ns.Text;
						target.IsNamespace = true;
						currentBuilder.NamespaceOrTypeNames.Add(target);
					}
					currentBuilder = target;
				}
				return currentBuilder;
			}

			public Builder GetContainerByName(string name, List<TokenIdentifier> generics)
			{
				var target = NamespaceOrTypeNames.FirstOrDefault(item => item.Name.Name == name && item.Name.Generics.Count == generics.Count);
				if (target == null) {
					target = new Builder();
					target.Name.Name = name;
					target.Name.Generics.AddRange(generics.Select(item => item.Text));
					target.IsNamespace = false;
					NamespaceOrTypeNames.Add(target);
				}
				return target;
			}

			public void AddType(InfoOutput infoOutput, UserTypeDeclare typeDeclare)
			{
				string name = typeDeclare.InstanceType.Name.Text;
				var generics = new List<TokenIdentifier>(typeDeclare.InstanceType.DeclaredGenerics.Select(item => item.Name));
				var classType = typeDeclare.InstanceType as ClassTypeDeclare;

				var existingType = Types.FirstOrDefault(item => item.Name.Name == name && item.Name.Generics.Count == generics.Count);
				if (existingType == null) {
					//Create new type declare
					var newType = new DeclaredManagedType.Builder();
					newType.Name.Name = name;
					newType.Name.Generics.AddRange(generics.Select(item => item.Text));
					newType.Items.Add(typeDeclare);
					Types.Add(newType);
				}
				else {
					//Add to existing as partial declare
					var existingClass = existingType.Items[0].InstanceType as ClassTypeDeclare;
					if (classType == null || existingClass == null || !classType.IsPartial || !existingClass.IsPartial) {
						infoOutput.OutputError(ParseException.AsToken(typeDeclare.InstanceType.Name, Error.Analysis.DuplicateTypeDeclare));
						return;
					}
					for (int cnt = 0; cnt < generics.Count; ++cnt) {
						if (generics[cnt].Text != existingClass.GenericTypes[cnt].Name.Text) {
							infoOutput.OutputError(ParseException.AsToken(generics[cnt], Error.Analysis.DifferGenericDeclare));
							return;
						}
					}
					existingType.Items.Add(typeDeclare);
				}

				//Process nested types
				if (classType != null) {
					var targetCollection = GetContainerByName(name, generics);
					foreach (var nestedType in classType.DefinedTypes) {
						targetCollection.AddType(infoOutput, nestedType);
					}
				}
			}
		}
	}
}
