
using System.Text.Json;

namespace Foo.Test;


public static class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello, World!");

        SimpleOneOf ff = 1;

           
        var gg = ff.Match<int>(
           onInteger: n => n ?? 0,
           onString: s => s.Length,
           onFoo: f => f.GetHashCode());






        var ops = new JsonSerializerOptions();
        ops.Converters.Add(new SimpleOneOfJsonSerializer());
        ops.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        ops.WriteIndented = true;

        var json = JsonSerializer.Serialize(ff, ops);


        var hhhh = JsonSerializer.Deserialize<SimpleOneOf>(json, ops);

    }







}



[SourceGenerator.UnionJsonConverterAttribute<SimpleOneOf>]
partial class SimpleOneOfJsonSerializer
{ }




[SourceGenerator.UnionOption<int>("IntVal")]
[SourceGenerator.UnionOption<bool>("BoolVal")]
public partial struct IntOrBool
{ }







[SourceGenerator.UnionOption<int?>("Integer")]
[SourceGenerator.UnionOption<string>("String")]
[SourceGenerator.UnionOption<Foo>("Foo")]
public partial struct SimpleOneOf
{ }


public record Foo(int I, string S);