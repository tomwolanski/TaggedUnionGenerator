using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Reflection;

namespace TaggedUnionGenerator.Tests.CodeGeneration
{
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