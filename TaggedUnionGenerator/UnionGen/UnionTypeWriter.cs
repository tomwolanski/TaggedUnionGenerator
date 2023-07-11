using Microsoft.CodeAnalysis;
using TaggedUnionGenerator.UnionGen;
using System.CodeDom.Compiler;
using System.Linq;

namespace TaggedUnionGenerator.Writers
{
    internal static class UnionTypeWriter
    {
        public static void Write(UnionTypeDefinition def, IndentedTextWriter writer)
        {
            using var _ = writer.StartBlock($"namespace {def.Namespace}");

            writer.WriteLine($"[{Consts.GeneratedCodeAttr}]");
            WriteStructDefinition(def, writer);
        }

        private  static void WriteStructDefinition(UnionTypeDefinition def, IndentedTextWriter writer)
        {
            using var _ = writer.StartBlock($"partial struct {def.Name} : global::SourceGenerator.IUnion<{def.Name}.TypeEnum>");

            WriteOptionEnums(def, writer);
            writer.WriteLine();

            writer.WriteLine("public TypeEnum Type { get; }");
            writer.WriteLine();

            WriteOptionFields(def, writer);
            writer.WriteLine();

            WriteConstructor(def, writer);
            writer.WriteLine();

            foreach (var option in def.Options)
            {
                WriteSingleFactoryMethod(def, option, writer);
                writer.WriteLine();
            }

            foreach (var option in def.Options)
            {
                WriteSingleCastOperator(def, option, writer);
                writer.WriteLine();
            }

            foreach (var option in def.Options)
            {
                WriteSingleTryGetMethod(def, option, writer);
                writer.WriteLine();
            }

            writer.WriteLine();

            WriteMatchMethod(def, writer);
            writer.WriteLine();

            WriteSwitchMethod(def, writer);
            writer.WriteLine();

            WriteGetClrTypeMethod(def, writer);
        }

        private static void WriteMatchMethod(UnionTypeDefinition def, IndentedTextWriter writer)
        {
            var ctorParameters = def.Options
                .Select(op => $"Func<{op.Type}, TResult> on{op.Name}")
                .JoinWithComa();

            using var _m = writer.StartBlock($"public TResult Match<TResult>({ctorParameters})");

            using var _s = writer.StartBlock($"return Type switch", close: "};");

            foreach (var op in def.Options)
            {
                writer.WriteLine($"TypeEnum.{op.Name} => on{op.Name}({op.Name.ToFieldNameCase()}),");
            }

            writer.WriteLine($"_ => throw new InvalidOperationException(\"Attempted to resolve invalid option type\")");
        }

        private static void WriteSwitchMethod(UnionTypeDefinition def, IndentedTextWriter writer)
        {
            var ctorParameters = def.Options
                .Select(op => $"Action<{op.Type}> on{op.Name}")
                .JoinWithComa();

            using var _m = writer.StartBlock($"public void Switch({ctorParameters})");

            using var _s = writer.StartBlock($"switch (Type)", close: "};");

            foreach (var op in def.Options)
            {
                writer.WriteLine($$"""case TypeEnum.{{op.Name}} : { on{{op.Name}}({{op.Name.ToFieldNameCase()}}); break; }""");
            }

            writer.WriteLine($"default: throw new InvalidOperationException(\"Attempted to resolve invalid option type\");");


        }

        private static void WriteGetClrTypeMethod(UnionTypeDefinition def, IndentedTextWriter writer)
        {
            using var _m = writer.StartBlock($"public static global::System.Type GetClrType(TypeEnum type)");

            using var _s = writer.StartBlock($"return type switch", close: "};");

            foreach (var op in def.Options)
            {
                writer.WriteLine($"TypeEnum.{op.Name} => typeof({op.Type}),");
            }

            writer.WriteLine($"_ => throw new InvalidOperationException(\"Attempted to resolve invalid option type\")");
        }

        private static void WriteConstructor(UnionTypeDefinition def, IndentedTextWriter writer)
        {
            var ctorParameters = def.Options
                .Select(op => $"{op.Type} {op.Name.ToCamelCase()}")
                .Prepend("TypeEnum type")
                .JoinWithComa();

            using var _ = writer.StartBlock($"private {def.Name}({ctorParameters})");

            writer.WriteLine($"Type = type;");

            foreach (var option in def.Options)
            {
                writer.WriteLine($"{option.Name.ToFieldNameCase()} = {option.Name.ToCamelCase()};");
            }
        }

        private static void WriteSingleFactoryMethod(UnionTypeDefinition def, UnionTypeOptionDefinition option, IndentedTextWriter writer)
        {
            var paramName = option.Name.ToCamelCase();

            using var _ = writer.StartBlock($"public static {def.Name} From{option.Name}({option.Type} {paramName})");

            var ctorParameters = def.Options
                .Select(op =>
                {
                    return op == option
                       ? paramName
                       : "default";
                })
                .Prepend($"TypeEnum.{option.Name}")
                .JoinWithComa();

            writer.WriteLine($"return new {def.Name}({ctorParameters});");
        }

        private static void WriteSingleCastOperator(UnionTypeDefinition def, UnionTypeOptionDefinition option, IndentedTextWriter writer)
        {
            var paramName = option.Name.ToCamelCase();

            var code = $"public static implicit operator {def.Name}({option.Type} {paramName}) => From{option.Name}({paramName});";

            writer.WriteLine(code);
        }

        private static void WriteSingleTryGetMethod(UnionTypeDefinition def, UnionTypeOptionDefinition option, IndentedTextWriter writer)
        {
            var paramName = option.Name.ToCamelCase();
            var fieldName = option.Name.ToFieldNameCase();

            using var _ = writer.StartBlock($"public bool TryGet{option.Name}(out {option.Type} {paramName})");

            using (writer.StartBlock($"if (Type == TypeEnum.{option.Name})"))
            {
                writer.WriteLine($"{paramName} = {fieldName};");
                writer.WriteLine($"return true;");
            }
            using (writer.StartBlock($"else"))
            {
                writer.WriteLine($"{paramName} = default;");
                writer.WriteLine($"return false;");
            }
        }

        private static void WriteOptionFields(UnionTypeDefinition def, IndentedTextWriter writer)
        {
            foreach (var option in def.Options)
            {
                var type = option.Type;
                var name = option.Name.ToFieldNameCase();

                writer.WriteLine($"private readonly {type} {name};");
            }
        }

        private static void WriteOptionEnums(UnionTypeDefinition def, IndentedTextWriter writer)
        {
            using var _ = writer.StartBlock("public enum TypeEnum");

            var pascalOptionNames = def.Options.Select(n => n.Name).ToArray();
            var lastOption = pascalOptionNames.Last();

            writer.WriteLine($"__Undefined,");

            foreach (var option in pascalOptionNames)
            {
                if (option != lastOption)
                {
                    writer.WriteLine($"{option},");
                }
                else
                {
                    writer.WriteLine($"{option}");
                }
            }
        }
    }

}

