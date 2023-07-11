using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Reflection;

namespace TaggedUnionGenerator.Tests
{
    public class TaggedUnionGeneratorTests
    {
        private readonly string Version = typeof(TaggedUnionGenerator).Assembly.GetName().Version.ToString();

        [Fact]
        public void ShouldGenerateCommonAttributes()
        {
            // language=csharp
            const string code = """
                namespace Ns
                { }
                """;

            var expectedUnionOptionAttributeCode = """
                // <auto-generated/>
                #nullable enable

                namespace SourceGenerator
                {
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("TaggedUnionGenerator", "1.0.0.0")]
                    [global::System.AttributeUsage(global::System.AttributeTargets.Struct, AllowMultiple = true)]
                    public sealed class UnionOptionAttribute<TUnionOption> : global::System.Attribute
                    {
                        public string OptionName { get;}
                        public Type OptionType => typeof(TUnionOption);

                        public UnionOptionAttribute(string name)
                        {
                            OptionName = name;
                        }
                    }
                }
                """;

            var expectedUnionCode = """
                // <auto-generated/>
                #nullable enable

                namespace SourceGenerator
                {
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("TaggedUnionGenerator", "1.0.0.0")]
                    public interface IUnion
                    { }
                }
                """;

            var expectedTypedUnionCode = """
                // <auto-generated/>
                #nullable enable

                namespace SourceGenerator
                {
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("TaggedUnionGenerator", "1.0.0.0")]
                    public interface IUnion<TTypeEnum> 
                        where TTypeEnum: struct, Enum
                    {
                        TTypeEnum Type { get; }
                    }
                }
                """;

            var runResult = GenTestHelpers.RunGenerator<TaggedUnionGenerator>(code);

            Assert.Empty(runResult.Diagnostics);
            Assert.True(runResult.ContainsExpectedCode(expectedUnionOptionAttributeCode));
            Assert.True(runResult.ContainsExpectedCode(expectedUnionCode));
            Assert.True(runResult.ContainsExpectedCode(expectedTypedUnionCode));
        }

        [Fact]
        public void ShouldGenerateUnionInterfaces()
        {
            // language=csharp
            const string code = """
                namespace Ns
                { }
                """;

            var expectedUntypedUnionCode = """
                // <auto-generated/>
                #nullable enable

                namespace SourceGenerator
                {
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("TaggedUnionGenerator", "1.0.0.0")]
                    public interface IUnion
                    { }
                }
                """;

            var expectedTypedUnionCode = """
                // <auto-generated/>
                #nullable enable

                namespace SourceGenerator
                {
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("TaggedUnionGenerator", "1.0.0.0")]
                    public interface IUnion<TTypeEnum> 
                        where TTypeEnum: struct, Enum
                    {
                        TTypeEnum Type { get; }
                    }
                }
                """;

            var runResult = GenTestHelpers.RunGenerator<TaggedUnionGenerator>(code);

            Assert.Empty(runResult.Diagnostics);
            Assert.True(runResult.ContainsExpectedCode(expectedUntypedUnionCode));
            Assert.True(runResult.ContainsExpectedCode(expectedTypedUnionCode));
        }

        [Fact]
        public void ShouldGenerateUnionJsonConverterAttributeAndConverterBase()
        {
            // language=csharp
            const string code = """
                namespace Ns
                { }
                """;

            var expectedAttributeCode = """
                // <auto-generated/>
                #nullable enable

                namespace SourceGenerator
                {
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("TaggedUnionGenerator", "1.0.0.0")]
                    [global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = false)]
                    public sealed class UnionJsonConverterAttribute<TUnion> : global::System.Attribute
                    { }
                }
                """;

            var expectedConverterBaseCode = """
                // <auto-generated/>
                #nullable enable

                namespace SourceGenerator
                {
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("TaggedUnionGenerator", "1.0.0.0")]
                    abstract class UnionJsonConverterBase<TUnion, TUnionTypeEnum> : global::System.Text.Json.Serialization.JsonConverter<TUnion>
                        where TUnionTypeEnum : struct, global::System.Enum
                        where TUnion : global::SourceGenerator.IUnion<TUnionTypeEnum>
                    {
                        const string TypePropName = "type";
                        const string DataPropName = "data";

                        public override TUnion Read(ref global::System.Text.Json.Utf8JsonReader reader, Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
                        {
                            if (reader.TokenType != global::System.Text.Json.JsonTokenType.StartObject)
                            {
                                throw new global::System.Text.Json.JsonException();
                            }

                            var type = ReadDataType(ref reader, options);

                            TUnion value = ReadUnionCore(ref reader, type, options);

                            if (reader.Read() && reader.TokenType == global::System.Text.Json.JsonTokenType.EndObject)
                            {
                                return value;
                            }

                            throw new global::System.Text.Json.JsonException();
                        }

                        protected abstract TUnion ReadUnionCore(ref global::System.Text.Json.Utf8JsonReader reader, TUnionTypeEnum type, global::System.Text.Json.JsonSerializerOptions options);

                        protected static TUnionTypeEnum ReadDataType(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Text.Json.JsonSerializerOptions options)
                        {
                            reader.Read();
                            AssertPropertyName(ref reader, TypePropName);

                            reader.Read();
                            var enumConverter = (global::System.Text.Json.Serialization.JsonConverter<TUnionTypeEnum>)options.GetConverter(typeof(TUnionTypeEnum));
                            return enumConverter.Read(ref reader, typeof(TUnionTypeEnum), options);
                        }

                        protected static T ReadDataData<T>(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Text.Json.JsonSerializerOptions options)
                        {
                            reader.Read();
                            AssertPropertyName(ref reader, DataPropName);

                            reader.Read();
                            var dataConverter = (global::System.Text.Json.Serialization.JsonConverter<T>)options.GetConverter(typeof(T));
                            return dataConverter.Read(ref reader, typeof(T), options);
                        }

                        static void AssertPropertyName(ref global::System.Text.Json.Utf8JsonReader reader, string expectedPropertyName)
                        {
                            if (reader.TokenType != global::System.Text.Json.JsonTokenType.PropertyName)
                            {
                                throw new global::System.Text.Json.JsonException($"Expected property \"{TypePropName}\".");
                            }

                            string? typePropertyName = reader.GetString();
                            if (typePropertyName != expectedPropertyName)
                            {
                                throw new global::System.Text.Json.JsonException($"Expected property \"{TypePropName}\".");
                            }
                        }

                        public override void Write(global::System.Text.Json.Utf8JsonWriter writer, TUnion value, global::System.Text.Json.JsonSerializerOptions options)
                        {
                            writer.WriteStartObject();

                            writer.WritePropertyName(TypePropName);

                            var enumConverter = (global::System.Text.Json.Serialization.JsonConverter<TUnionTypeEnum>)options.GetConverter(typeof(TUnionTypeEnum));
                            enumConverter.Write(writer, value.Type, options);

                            WriteUnionCore(writer, value, options);

                            writer.WriteEndObject();
                        }

                        protected abstract void WriteUnionCore(global::System.Text.Json.Utf8JsonWriter writer, TUnion value, global::System.Text.Json.JsonSerializerOptions options);

                        protected static void WriteData<T>(global::System.Text.Json.Utf8JsonWriter writer, T v, global::System.Text.Json.JsonSerializerOptions options)
                        {
                            writer.WritePropertyName(DataPropName);

                            var dataConverter = (global::System.Text.Json.Serialization.JsonConverter<T>)options.GetConverter(typeof(T));
                            dataConverter.Write(writer, v, options);
                        }
                    }
                }
                """;

            var runResult = GenTestHelpers.RunGenerator<TaggedUnionGenerator>(code);

            Assert.Empty(runResult.Diagnostics);
            Assert.True(runResult.ContainsExpectedCode(expectedAttributeCode));
            Assert.True(runResult.ContainsExpectedCode(expectedConverterBaseCode));
        }

        [Fact]
        public void ShouldGenerateValidIntOrBoolUnion()
        {
            // language=csharp
            const string code = """
                namespace Ns
                {
                    [SourceGenerator.UnionOption<int>("IntVal")]
                    [SourceGenerator.UnionOption<bool>("BoolVal")]
                    public partial struct IntOrBool
                    { }
                }
                """;

            var expectedCode = """
                // <auto-generated/>
                #nullable enable
                namespace Ns
                {
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("TaggedUnionGenerator", "1.0.0.0")]
                    partial struct IntOrBool : global::SourceGenerator.IUnion<IntOrBool.TypeEnum>
                    {
                        public enum TypeEnum
                        {
                            __Undefined,
                            IntVal,
                            BoolVal
                        }

                        public TypeEnum Type { get; }

                        private readonly int _intVal;
                        private readonly bool _boolVal;

                        private IntOrBool(TypeEnum type, int intVal, bool boolVal)
                        {
                            Type = type;
                            _intVal = intVal;
                            _boolVal = boolVal;
                        }

                        public static IntOrBool FromIntVal(int intVal)
                        {
                            return new IntOrBool(TypeEnum.IntVal, intVal, default);
                        }

                        public static IntOrBool FromBoolVal(bool boolVal)
                        {
                            return new IntOrBool(TypeEnum.BoolVal, default, boolVal);
                        }

                        public static implicit operator IntOrBool(int intVal) => FromIntVal(intVal);

                        public static implicit operator IntOrBool(bool boolVal) => FromBoolVal(boolVal);

                        public bool TryGetIntVal(out int intVal)
                        {
                            if (Type == TypeEnum.IntVal)
                            {
                                intVal = _intVal;
                                return true;
                            }
                            else
                            {
                                intVal = default;
                                return false;
                            }
                        }

                        public bool TryGetBoolVal(out bool boolVal)
                        {
                            if (Type == TypeEnum.BoolVal)
                            {
                                boolVal = _boolVal;
                                return true;
                            }
                            else
                            {
                                boolVal = default;
                                return false;
                            }
                        }


                        public TResult Match<TResult>(Func<int, TResult> onIntVal, Func<bool, TResult> onBoolVal)
                        {
                            return Type switch
                            {
                                TypeEnum.IntVal => onIntVal(_intVal),
                                TypeEnum.BoolVal => onBoolVal(_boolVal),
                                _ => throw new InvalidOperationException("Attempted to resolve invalid option type")
                            };
                        }

                        public void Switch(Action<int> onIntVal, Action<bool> onBoolVal)
                        {
                            switch (Type)
                            {
                                case TypeEnum.IntVal : { onIntVal(_intVal); break; }
                                case TypeEnum.BoolVal : { onBoolVal(_boolVal); break; }
                                default: throw new InvalidOperationException("Attempted to resolve invalid option type");
                            };
                        }

                        public static global::System.Type GetClrType(TypeEnum type)
                        {
                            return type switch
                            {
                                TypeEnum.IntVal => typeof(int),
                                TypeEnum.BoolVal => typeof(bool),
                                _ => throw new InvalidOperationException("Attempted to resolve invalid option type")
                            };
                        }
                    }
                }
                """;

            var runResult = GenTestHelpers.RunGenerator<TaggedUnionGenerator>(code);

            Assert.Empty(runResult.Diagnostics);
            Assert.True(runResult.ContainsExpectedCode(expectedCode));
        }

        [Fact]
        public void ShouldGenerateValidUnionWithComplexTypes()
        {
            // language=csharp
            const string code = """
                namespace Ns
                {
                    public record ComplexType1(int IntVal, string StringVal);
                    public record ComplexType2(Guid GuidVal);

                    [SourceGenerator.UnionOption<int>("IntVal")]
                    [SourceGenerator.UnionOption<bool>("BoolVal")]
                    [SourceGenerator.UnionOption<ComplexType1>("ComplexType1Val")]
                    [SourceGenerator.UnionOption<ComplexType2>("ComplexType2Val")]
                    public partial struct IntOrBool
                    { }
                }
                """;

            var expectedCode = """
                //<auto-generated/>
                #nullable enable
                namespace Ns
                {
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("TaggedUnionGenerator", "1.0.0.0")]
                    partial struct IntOrBool : global::SourceGenerator.IUnion<IntOrBool.TypeEnum>
                    {
                        public enum TypeEnum
                        {
                            __Undefined,
                            IntVal,
                            BoolVal,
                            ComplexType1Val,
                            ComplexType2Val
                        }

                        public TypeEnum Type { get; }

                        private readonly int _intVal;
                        private readonly bool _boolVal;
                        private readonly global::Ns.ComplexType1 _complexType1Val;
                        private readonly global::Ns.ComplexType2 _complexType2Val;

                        private IntOrBool(TypeEnum type, int intVal, bool boolVal, global::Ns.ComplexType1 complexType1Val, global::Ns.ComplexType2 complexType2Val)
                        {
                            Type = type;
                            _intVal = intVal;
                            _boolVal = boolVal;
                            _complexType1Val = complexType1Val;
                            _complexType2Val = complexType2Val;
                        }

                        public static IntOrBool FromIntVal(int intVal)
                        {
                            return new IntOrBool(TypeEnum.IntVal, intVal, default, default, default);
                        }

                        public static IntOrBool FromBoolVal(bool boolVal)
                        {
                            return new IntOrBool(TypeEnum.BoolVal, default, boolVal, default, default);
                        }

                        public static IntOrBool FromComplexType1Val(global::Ns.ComplexType1 complexType1Val)
                        {
                            return new IntOrBool(TypeEnum.ComplexType1Val, default, default, complexType1Val, default);
                        }

                        public static IntOrBool FromComplexType2Val(global::Ns.ComplexType2 complexType2Val)
                        {
                            return new IntOrBool(TypeEnum.ComplexType2Val, default, default, default, complexType2Val);
                        }

                        public static implicit operator IntOrBool(int intVal) => FromIntVal(intVal);

                        public static implicit operator IntOrBool(bool boolVal) => FromBoolVal(boolVal);

                        public static implicit operator IntOrBool(global::Ns.ComplexType1 complexType1Val) => FromComplexType1Val(complexType1Val);

                        public static implicit operator IntOrBool(global::Ns.ComplexType2 complexType2Val) => FromComplexType2Val(complexType2Val);

                        public bool TryGetIntVal(out int intVal)
                        {
                            if (Type == TypeEnum.IntVal)
                            {
                                intVal = _intVal;
                                return true;
                            }
                            else
                            {
                                intVal = default;
                                return false;
                            }
                        }

                        public bool TryGetBoolVal(out bool boolVal)
                        {
                            if (Type == TypeEnum.BoolVal)
                            {
                                boolVal = _boolVal;
                                return true;
                            }
                            else
                            {
                                boolVal = default;
                                return false;
                            }
                        }

                        public bool TryGetComplexType1Val(out global::Ns.ComplexType1 complexType1Val)
                        {
                            if (Type == TypeEnum.ComplexType1Val)
                            {
                                complexType1Val = _complexType1Val;
                                return true;
                            }
                            else
                            {
                                complexType1Val = default;
                                return false;
                            }
                        }

                        public bool TryGetComplexType2Val(out global::Ns.ComplexType2 complexType2Val)
                        {
                            if (Type == TypeEnum.ComplexType2Val)
                            {
                                complexType2Val = _complexType2Val;
                                return true;
                            }
                            else
                            {
                                complexType2Val = default;
                                return false;
                            }
                        }


                        public TResult Match<TResult>(Func<int, TResult> onIntVal, Func<bool, TResult> onBoolVal, Func<global::Ns.ComplexType1, TResult> onComplexType1Val, Func<global::Ns.ComplexType2, TResult> onComplexType2Val)
                        {
                            return Type switch
                            {
                                TypeEnum.IntVal => onIntVal(_intVal),
                                TypeEnum.BoolVal => onBoolVal(_boolVal),
                                TypeEnum.ComplexType1Val => onComplexType1Val(_complexType1Val),
                                TypeEnum.ComplexType2Val => onComplexType2Val(_complexType2Val),
                                _ => throw new InvalidOperationException("Attempted to resolve invalid option type")
                            };
                        }

                        public void Switch(Action<int> onIntVal, Action<bool> onBoolVal, Action<global::Ns.ComplexType1> onComplexType1Val, Action<global::Ns.ComplexType2> onComplexType2Val)
                        {
                            switch (Type)
                            {
                                case TypeEnum.IntVal : { onIntVal(_intVal); break; }
                                case TypeEnum.BoolVal : { onBoolVal(_boolVal); break; }
                                case TypeEnum.ComplexType1Val : { onComplexType1Val(_complexType1Val); break; }
                                case TypeEnum.ComplexType2Val : { onComplexType2Val(_complexType2Val); break; }
                                default: throw new InvalidOperationException("Attempted to resolve invalid option type");
                            };
                        }

                        public static global::System.Type GetClrType(TypeEnum type)
                        {
                            return type switch
                            {
                                TypeEnum.IntVal => typeof(int),
                                TypeEnum.BoolVal => typeof(bool),
                                TypeEnum.ComplexType1Val => typeof(global::Ns.ComplexType1),
                                TypeEnum.ComplexType2Val => typeof(global::Ns.ComplexType2),
                                _ => throw new InvalidOperationException("Attempted to resolve invalid option type")
                            };
                        }
                    }
                }
                """;

            var runResult = GenTestHelpers.RunGenerator<TaggedUnionGenerator>(code);

            Assert.Empty(runResult.Diagnostics);
            Assert.True(runResult.ContainsExpectedCode(expectedCode));
        }




        [Fact]
        public void ShouldRaiseErrorOnDuplicatedUnionOptionName()
        {
            // language=csharp
            const string code = """
                namespace Ns
                {
                    [SourceGenerator.UnionOption<int>("IntVal")]
                    [SourceGenerator.UnionOption<bool>("IntVal")]
                    public partial struct IntOrBool
                    { }
                }
                """;

            var runResult = GenTestHelpers.RunGenerator<TaggedUnionGenerator>(code);

            Assert.NotEmpty(runResult.Diagnostics);
            Assert.Collection(runResult.Diagnostics,
                n => n.AssertDiagnosticEquals(DiagnosticSeverity.Error, "UG0003", "Failed to generate union based on type Ns.IntOrBool.  Option name 'IntVal' was user more than once."));
        }


        [Fact]
        public void ShouldRaiseErrorOnEmptyOptionName()
        {
            // language=csharp
            const string code = """
                namespace Ns
                {
                    [SourceGenerator.UnionOption<int>("")]
                    [SourceGenerator.UnionOption<bool>("IntVal")]
                    public partial struct IntOrBool
                    { }
                }
                """;

            var runResult = GenTestHelpers.RunGenerator<TaggedUnionGenerator>(code);

            Assert.NotEmpty(runResult.Diagnostics);
            Assert.Collection(runResult.Diagnostics,
                n => n.AssertDiagnosticEquals(DiagnosticSeverity.Error, "UG0002", "Failed to generate union based on type Ns.IntOrBool. Union option name cannot me empty."));
        }

        [Fact]
        public void ShouldRaiseErrorOnNullOptionName()
        {
            // language=csharp
            const string code = """
                namespace Ns
                {
                    [SourceGenerator.UnionOption<int>(null)]
                    [SourceGenerator.UnionOption<bool>("IntVal")]
                    public partial struct IntOrBool
                    { }
                }
                """;

            var runResult = GenTestHelpers.RunGenerator<TaggedUnionGenerator>(code);

            Assert.Collection(runResult.Diagnostics,
                n => n.AssertDiagnosticEquals(DiagnosticSeverity.Error, "UG0002", "Failed to generate union based on type Ns.IntOrBool. Union option name cannot me empty."));
        }

        [Fact]
        public void ShouldRaiseWarningOnUnionWithSingleOption()
        {
            // language=csharp
            const string code = """
                namespace Ns
                {
                    [SourceGenerator.UnionOption<bool>("IntVal")]
                    public partial struct IntOrBool
                    { }
                }
                """;

            var runResult = GenTestHelpers.RunGenerator<TaggedUnionGenerator>(code);

            Assert.Collection(runResult.Diagnostics,
                n => n.AssertDiagnosticEquals(DiagnosticSeverity.Warning, "UG0005", "Union based on type Ns.IntOrBool has a single option and serves no purpose."));
        }
    }

    public static class GenTestHelpers
    {
        public static GeneratorDriverRunResult RunGenerator<TGenerator>(string code)
            where TGenerator : IIncrementalGenerator, new()
            => RunGenerator<TGenerator>(CSharpSyntaxTree.ParseText(code));

        public static GeneratorDriverRunResult RunGenerator<TGenerator>(SyntaxTree syntaxTree)
            where TGenerator : IIncrementalGenerator, new()
        {
            var compilation = CSharpCompilation.Create("compilation",
                new[] { syntaxTree },
                new[] { MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new TGenerator();

            return CSharpGeneratorDriver.Create(generator)
                .RunGeneratorsAndUpdateCompilation(compilation, out var _, out var _)
                .GetRunResult();
        }

        public static bool ContainsExpectedCode(this GeneratorDriverRunResult result, string expectedCode)
            => ContainsExpectedSyntaxTree(result, CSharpSyntaxTree.ParseText(expectedCode));

        public static bool ContainsExpectedSyntaxTree(this GeneratorDriverRunResult result, SyntaxTree expectedSyntaxTree)
            => result.GeneratedTrees.Any(t => t.IsEquivalentTo(expectedSyntaxTree));


        public static void AssertDiagnosticEquals(this Diagnostic diagnostic, DiagnosticSeverity expectedDiagnosticSeverity, string expectedId, string expectedMessage)
        {
            Assert.Equal(expectedDiagnosticSeverity, diagnostic.Severity);
            Assert.Equal(expectedId, diagnostic.Id);
            Assert.Contains(expectedMessage, diagnostic.ToString());
        }

    }


}