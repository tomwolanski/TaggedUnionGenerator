using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace TaggedUnionGenerator
{
    internal static class Ex
    {
        public static IDisposable StartBlock(this IndentedTextWriter writer, string? text, string open = "{", string close = "}")
        {
            if (text is not null)
            {
                writer.WriteLine(text);
            }
            if (open is not null)
            {
                writer.WriteLine(open);
            }

            writer.Indent++;

            return new ActionDisposable(() =>
            {
                writer.Indent--;

                if (close is not null)
                {
                    writer.WriteLine(close);
                }
            });
        }

        public static string ToPascalCase(this string val)
        {
            var info = CultureInfo.InvariantCulture.TextInfo;
            
            return info.ToTitleCase(val);
        }

        public static string ToCamelCase(this string val)
        {
            val = char.ToLower(val[0]) + val.Substring(1);

            var keywords = new[] { "struct", "class", "enum", "string", "int", "float", "short", "double", "global", "default", "this" };
            if (keywords.Contains(val))
            {
                val = '@' + val;
            }

            return val;
        }

        public static string ToFieldNameCase(this string val)
        {
            val = char.ToLower(val[0]) + val.Substring(1);

            return $"_{val}";
        }

        public static string JoinWithComa(this IEnumerable<string> values)
        {
            return string.Join(", ", values);
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> values, T value)
        {
            return Enumerable.Concat(
                new[] { value },
                values);
        }

    }

}

