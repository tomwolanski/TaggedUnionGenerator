using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace TaggedUnionGenerator.UnionGen
{
    record UnionTypeDefinition(string? Namespace, string Name, bool GenerateCastOperators, ImmutableArray<UnionTypeOptionDefinition> Options, Location? Location);

}

