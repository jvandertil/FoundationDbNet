namespace FoundationDbNet.Tests.Features.Layers.Tuple
{
    using FoundationDbNet.Layers.Tuple;
    using FoundationDbNet.Tests.Framework.Layers.Tuple;
    using Xunit;

    public class BooleanEncoderTests : EncoderTestsBase<bool>
    {
        public BooleanEncoderTests()
            : base(new BooleanEncoder())
        {
        }

        [Theory]
        [InlineData(true, "0x27")]
        [InlineData(false, "0x26")]
        public void CanEncodeBoolean(bool value, string expected)
            => EncodeAndVerify(value, expected);
    }
}
