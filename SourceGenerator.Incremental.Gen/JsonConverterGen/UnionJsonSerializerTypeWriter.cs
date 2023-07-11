using System.CodeDom.Compiler;
using System.Linq;
using SourceGenerator.Incremental.Gen.UnionGen;

namespace SourceGenerator.Incremental.Gen.JsonConverterGen
{
    internal static class UnionJsonSerializerTypeWriter
    {
        public static void Write(UnionTypeJsonConverterDefinition def, IndentedTextWriter writer)
        {
            using var _ = writer.StartBlock($"namespace {def.Namespace}");

            writer.WriteLine($"[{Consts.GeneratedCodeAttr}]");
            WriteClassDefinition(def, writer);
        }

        private static void WriteClassDefinition(UnionTypeJsonConverterDefinition def, IndentedTextWriter writer)
        {
            var unionType = $"global::{def.UnionDefinition.Namespace}.{def.UnionDefinition.Name}";

            using var _ = writer.StartBlock($"partial class {def.Name} : global::SourceGenerator.UnionJsonConverterBase<{unionType}, {unionType}.TypeEnum>");


            WriteReadUnionCoreMethod(def.UnionDefinition, writer);
            writer.WriteLine();

            WriteWriteUnionCoreMethod(def.UnionDefinition, writer);
            writer.WriteLine();
        }

        private static void WriteReadUnionCoreMethod(UnionTypeDefinition def, IndentedTextWriter writer)
        {
            using var _m = writer.StartBlock($"protected override global::{def.Namespace}.{def.Name} ReadUnionCore(ref global::System.Text.Json.Utf8JsonReader reader, global::{def.Namespace}.{def.Name}.TypeEnum type, global::System.Text.Json.JsonSerializerOptions options)");

            using var _s = writer.StartBlock($"return type switch", close: "};");

            foreach (var op in def.Options)
            {
                writer.WriteLine($"global::{def.Namespace}.{def.Name}.TypeEnum.{op.Name} => ReadDataData<{op.Type}>(ref reader, options),");
            }

            writer.WriteLine($"_ => throw new InvalidOperationException(\"Attempted to resolve invalid option type\")");
        }

        private static void WriteWriteUnionCoreMethod(UnionTypeDefinition def, IndentedTextWriter writer)
        {
            using var _m = writer.StartBlock($"protected override void WriteUnionCore(global::System.Text.Json.Utf8JsonWriter writer, global::{def.Namespace}.{def.Name} value, global::System.Text.Json.JsonSerializerOptions options)");

            using var _s = writer.StartBlock($"value.Switch(", open: string.Empty, close: string.Empty);

            var lastOption = def.Options.Last();
            foreach (var op in def.Options)
            {
                var end = op == lastOption
                    ? ");"
                    : ",";

                writer.WriteLine($"""on{op.Name}: v => WriteData(writer, v, options){end}""");
            }
        }

    }

}

