# TaggedUnionGenerator

A simple code generator to produce a tagged discriminated unions.

Example usage:
```csharp
namespace NS;

using TaggedUnion;
using TaggedUnion.Json;


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
```


## VS linting support

## Detect empty option name.
All options require name, so the values like empty string or null cannot be provided.
![image](https://github.com/tomwolanski/TaggedUnionGenerator/assets/68085653/6edf4aae-203f-47cc-bbfa-0253b9ca1a14)


### Detect multiple options using the sme name.
In this scenario the generator is unable to produce methods with the same name, so an error is produced:
![image](https://github.com/tomwolanski/TaggedUnionGenerator/assets/68085653/cc0e941a-400a-4461-9deb-f239720e70db)


### Detect multiple options using the same CRL type.
In this scenario the generator skips generation of cast operators since wo would not be able to decide whenever the value should be matched to which factory method.
![image](https://github.com/tomwolanski/TaggedUnionGenerator/assets/68085653/8af77b5e-f549-489f-a25e-65f6c024bcfa)


### Detect invalid usage of JSON converter attrubute.
A `UnionJsonConverterAttribute<>` requires a type to be a valid union with options, so invalid usage fails the build.
![image](https://github.com/tomwolanski/TaggedUnionGenerator/assets/68085653/d9298d32-8027-4ca7-bcf9-96405f20a454)




