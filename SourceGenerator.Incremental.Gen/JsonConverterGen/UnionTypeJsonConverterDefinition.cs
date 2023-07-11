using Microsoft.CodeAnalysis;
using SourceGenerator.Incremental.Gen.UnionGen;

namespace SourceGenerator.Incremental.Gen.JsonConverterGen
{
    record UnionTypeJsonConverterDefinition(string? Namespace, string Name, UnionTypeDefinition UnionDefinition, Location? Location);

}

