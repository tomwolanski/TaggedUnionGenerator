using System.Text.Json;

using TaggedUnion;
using TaggedUnion.Json;

namespace NS;


// define a union with 2 options. 
[UnionOption<int>("IntegerVal")]
[UnionOption<bool>("BooleanVal")]
public partial struct IntOrBool
{ }


// optionally, define a System.Text.Json converter class
[UnionJsonConverterAttribute<IntOrBool>]
public partial class IntOrBoolJsonSerializer
{ }


public static class Program
{
    public static void Main()
    {
        // create a union with factory method
        IntOrBool value = IntOrBool.FromBooleanVal(false);


        // create a union using implicit cast operator
        value = 2;


        // test whitch option the union stores
        if (value.Type == IntOrBool.TypeEnum.BooleanVal)
        {
            Console.WriteLine($"it is a boolean!");
        }


        // transform value of the union
        var matched = value.Match<string>(
            onBooleanVal: v => $"got boolean value {v}",
            onIntegerVal: v => $"got integer value {v}");

        Console.WriteLine(matched);


        // act on the value of the union
        value.Switch(
            onBooleanVal: v => Console.WriteLine($"got boolean value {v}"),
            onIntegerVal: v => Console.WriteLine($"got integer value {v}"));


        // attempt to pick one of the option
        if (value.TryGetIntegerVal(out var v))
        {
            Console.WriteLine($"it is an integer of value {v}!");
        }


        // Json serialization using System.Text.Json
        var ops = new JsonSerializerOptions();
        ops.Converters.Add(new IntOrBoolJsonSerializer()); // new serializer that is generated
        ops.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());  // current union type is stored as an enum, so we can serialize it as a string too;
        ops.WriteIndented = true;

        var json = JsonSerializer.Serialize(value, ops);
        var deserialized = JsonSerializer.Deserialize<IntOrBool>(json, ops);

    }
}


