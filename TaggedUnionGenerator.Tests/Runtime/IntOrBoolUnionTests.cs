using TaggedUnion;

namespace TaggedUnionGenerator.Tests
{
    [UnionOption<int>("IntegerVal")]
    [UnionOption<bool>("BooleanVal")]
    public partial struct IntOrBool
    { }

    public class IntOrBoolUnionTests
    {
        [Fact]
        public void FromBooleanVal_ShouldProduceValidBoolean()
        {
            var target = IntOrBool.FromBooleanVal(false);

            Assert.True(target.Type == IntOrBool.TypeEnum.BooleanVal);
            Assert.False(target.Type == IntOrBool.TypeEnum.IntegerVal);
        }

        [Fact]
        public void FromIntegerVal_ShouldProduceValidInteger()
        {
            var target = IntOrBool.FromIntegerVal(0);

            Assert.True(target.Type == IntOrBool.TypeEnum.IntegerVal);
            Assert.False(target.Type == IntOrBool.TypeEnum.BooleanVal);
        }

        [Fact]
        public void CastOperator_ShouldProduceValidBoolean()
        {
            IntOrBool target = false;

            Assert.True(target.Type == IntOrBool.TypeEnum.BooleanVal);
            Assert.False(target.Type == IntOrBool.TypeEnum.IntegerVal);
        }

        [Fact]
        public void CastOperator_ShouldProduceValidInteger()
        {
            IntOrBool target = 0;

            Assert.True(target.Type == IntOrBool.TypeEnum.IntegerVal);
            Assert.False(target.Type == IntOrBool.TypeEnum.BooleanVal);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TryGetIntegerVal_ShouldReturnProperValuesForBooleanValue(bool value)
        {
            IntOrBool target = value;

            Assert.True(target.TryGetBooleanVal(out var resBool));
            Assert.Equal(value, resBool);

            Assert.False(target.TryGetIntegerVal(out var resInt));
            Assert.Equal(default, resInt);
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        [InlineData(5)]
        public void TryGetIntegerVal_ShouldReturnProperValuesForIntegerValue(int value)
        {
            IntOrBool target = value;

            Assert.True(target.TryGetIntegerVal(out var resInt));
            Assert.Equal(value, resInt);

            Assert.False(target.TryGetBooleanVal(out var resBool));
            Assert.Equal(default, resBool);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Switch_ShouldExecuteForBooleanValue(bool value)
        {
            IntOrBool target = value;

            var wasCalled = false;

            target.Switch(
                onIntegerVal: _ =>
                {
                    Assert.Fail("wrong branch executed");
                },
                onBooleanVal: v =>
                {
                    wasCalled = true;
                    Assert.Equal(value, v);
                });

            Assert.True(wasCalled);
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        [InlineData(5)]
        public void Switch_ShouldExecuteForIntegerValue(int value)
        {
            IntOrBool target = value;

            var wasCalled = false;

            target.Switch(
                onIntegerVal: v =>  
                {
                    wasCalled = true;
                    Assert.Equal(value, v);
                },
                onBooleanVal: _ => 
                {
                    Assert.Fail("wrong branch executed");
                });

            Assert.True(wasCalled);
        }

        [Theory]
        [InlineData(true, "bool True")]
        [InlineData(false, "bool False")]
        public void Match_ShouldExecuteForBooleanValue(bool value, string expectedResult)
        {
            IntOrBool target = value;

            var result =  target.Match<string>(
                onIntegerVal: v => $"int {v}",
                onBooleanVal: v => $"bool {v}");

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(-5, "int -5")]
        [InlineData(0, "int 0")]
        [InlineData(5, "int 5")]
        public void Match_ShouldExecuteForIntegerValue(int value, string expectedResult)
        {
            IntOrBool target = value;

            var result = target.Match<string>(
                onIntegerVal: v => $"int {v}",
                onBooleanVal: v => $"bool {v}");

            Assert.Equal(expectedResult, result);
        }
    }
}