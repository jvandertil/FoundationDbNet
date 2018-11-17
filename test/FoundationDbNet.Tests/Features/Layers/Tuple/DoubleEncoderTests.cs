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
        [InlineData(double.PositiveInfinity, "0X21FFF8000000000001")]
        [InlineData(10.101, "0X21C02433B645A1CAC1")]
        public void CanEncodeDouble(double value, string expected)
            => EncodeAndVerify(value, expected);
    }
}
