using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace TaggedUnionGenerator.UnionGen
{
    record UnionTypeDefinition(string? Namespace, string Name, ImmutableArray<UnionTypeOptionDefinition> Options, Location? Location);

}

