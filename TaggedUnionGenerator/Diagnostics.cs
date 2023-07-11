using Microsoft.CodeAnalysis;

namespace TaggedUnionGenerator
{
    static class Diagnostics
    {
        public static readonly DiagnosticDescriptor ErrorUnionGenerationGenericError = new DiagnosticDescriptor(
                        "UG0001",
                        "Failed to generate union struct",
                        "Failed to generate union based on type {0}.{1}. Exception was raised: '{2}'",
                        Consts.AssemblyName.Name,
                        DiagnosticSeverity.Error,
                        true);

        public static readonly DiagnosticDescriptor ErrorEmptyOptionName = new DiagnosticDescriptor(
                        "UG0002",
                        "Union option name cannot me empty",
                        "Failed to generate union based on type {0}.{1}. Union option name cannot me empty.",
                        Consts.AssemblyName.Name,
                        DiagnosticSeverity.Error,
                        true);

        public static readonly DiagnosticDescriptor ErrorOptionNameUsedTwice = new DiagnosticDescriptor(
                        "UG0003",
                        "Union option names are not unique",
                        "Failed to generate union based on type {0}.{1}.  Option name '{2}' was user more than once.",
                        Consts.AssemblyName.Name,
                        DiagnosticSeverity.Error,
                        true);

        public static readonly DiagnosticDescriptor ErrorInvalidOptionNameFormat = new DiagnosticDescriptor(
                        "UG0004",
                        "Union option name does not start with capital letter.",
                        "Failed to generate union based on type {0}.{1}. Option name '{2}' should start with capital letter.",
                        Consts.AssemblyName.Name,
                        DiagnosticSeverity.Error,
                        true);

        public static readonly DiagnosticDescriptor ErrorSingleOptionServerNoPurpose = new DiagnosticDescriptor(
                        "UG0005",
                        "Union with single option serves no purpose",
                        "Union based on type {0}.{1} has a single option and serves no purpose.",
                        Consts.AssemblyName.Name,
                        DiagnosticSeverity.Warning,
                        true);

        public static readonly DiagnosticDescriptor ErrorTypeIsNotAnUnion = new DiagnosticDescriptor(
                        "UG0006",
                        "Type is not an union",
                        "Type {0}.{1} has no options and is not an union.",
                        Consts.AssemblyName.Name,
                        DiagnosticSeverity.Error,
                        true);

        public static readonly DiagnosticDescriptor ErrorSerializerGenerationGenericError = new DiagnosticDescriptor(
                        "UG0007",
                        "Failed to generate union json serializer",
                        "Failed to generate union json serializer based on type {0}.{1}. Exception was raised: '{2}'",
                        Consts.AssemblyName.Name,
                        DiagnosticSeverity.Error,
                        true);

        
    }
}

