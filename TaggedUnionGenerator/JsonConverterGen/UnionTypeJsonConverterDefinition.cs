using Microsoft.CodeAnalysis;
using TaggedUnionGenerator.UnionGen;

namespace TaggedUnionGenerator.JsonConverterGen
{
    record UnionTypeJsonConverterDefinition(string? Namespace, string Name, UnionTypeDefinition UnionDefinition, Location? Location);

}

