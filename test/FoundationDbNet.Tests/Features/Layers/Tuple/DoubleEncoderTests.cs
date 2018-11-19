using FoundationDbNet.Layers.Tuple;
using Xunit;

namespace FoundationDbNet.Tests.Features.Layers.Tuple
{
    using FoundationDbNet.Tests.Framework.Layers.Tuple;

    public class DoubleEncoderTests : EncoderTestsBase<double>
    {
        public DoubleEncoderTests()
        : base(new DoubleEncoder())
        {
        }

        [Theory]
        [InlineData(double.NaN, "0X210007FFFFFFFFFFFF")]
        [InlineData(10.101, "0X21C02433B645A1CAC1")]
        [InlineData(10, "0x21c024000000000000")]
        [InlineData(1.12333121337e+07, "0x21c1656d0404474539")]
        public void CanEncodeDouble(double value, string expected)
            => EncodeAndVerify(value, expected);
    }
}
