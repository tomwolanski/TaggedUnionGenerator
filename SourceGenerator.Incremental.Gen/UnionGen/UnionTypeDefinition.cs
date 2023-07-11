using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace SourceGenerator.Incremental.Gen.UnionGen
{
    record UnionTypeDefinition(string? Namespace, string Name, ImmutableArray<UnionTypeOptionDefinition> Options, Location? Location);

}

