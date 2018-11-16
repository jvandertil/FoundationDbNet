namespace FoundationDbNet.Tests.Features.Layers.Tuple
{
    using FoundationDbNet.Layers.Tuple;
    using FoundationDbNet.Tests.Framework.Layers.Tuple;
    using Xunit;

    public class StringEncoderTests : EncoderTestsBase<string>
    {
        public StringEncoderTests()
            : base(new StringEncoder())
        {
        }

        [Theory]
        [InlineData(null, "0x00")]
        [InlineData("", "0x0200")]
        [InlineData("FOObar", "0x02464f4f62617200")]
        [InlineData("F\u00d4O\u0000bar", "0x0246c3944f00ff62617200")]
        public void CanEncodeString(string value, string expected)
            => EncodeAndVerify(value, expected);
    }
}
