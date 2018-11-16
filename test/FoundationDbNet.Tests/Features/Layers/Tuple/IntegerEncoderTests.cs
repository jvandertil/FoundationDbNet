namespace FoundationDbNet.Tests.Features.Layers.Tuple
{
    using FoundationDbNet.Layers.Tuple;
    using FoundationDbNet.Tests.Framework.Layers.Tuple;
    using Xunit;

    public class IntegerEncoderTests : EncoderTestsBase<long>
    {
        public IntegerEncoderTests()
            : base(new IntegerEncoder())
        {
        }

        [Theory]
        [InlineData(0, "0x14")] // Technically not positive, but whatever ;).
        [InlineData(128, "0x1580")]
        [InlineData(0xFF, "0x15FF")]
        [InlineData(16801, "0x1641A1")]
        [InlineData(0xFFFF, "0x16FFFF")]
        [InlineData(14727200, "0x17E0B820")]
        [InlineData(0xFFFFFF, "0x17FFFFFF")]
        [InlineData(1455720000, "0x1856C48640")]
        [InlineData(0xFFFFFFFF, "0x18FFFFFFFF")]
        [InlineData(0x7FFFFFFFFFFFFFFF, "0x1C7FFFFFFFFFFFFFFF")]

        public void CanEncodePositiveLongValues(long value, string expected)
            => EncodeAndVerify(value, expected);

        [Theory]
        [InlineData(-1, "0x13FE")]
        [InlineData(-128, "0x137F")]
        [InlineData(-0xFF, "0x1300")]
        [InlineData(-16801, "0x12BE5E")]
        [InlineData(-0xFFFF, "0x120000")]
        [InlineData(-14727200, "0x111F47DF")]
        [InlineData(-0xFFFFFF, "0x11000000")]
        [InlineData(-1455720000, "0X10A93B79BF")]
        [InlineData(-0xFFFFFFFF, "0x1000000000")]
        [InlineData(-6273172034854775807, "0X0CA8F137455F911400")]
        [InlineData(-9223372036854775808, "0X0C7FFFFFFFFFFFFFFF")]
        public void CanEncodeNegativeLongValues(long value, string expected)
            => EncodeAndVerify(value, expected);
    }

}
