using FoundationDbNet.Layers.Tuple;
using Xunit;

namespace FoundationDbNet.Tests.Features.Layers.Tuple
{
    using FoundationDbNet.Tests.Framework.Layers.Tuple;

    public class FloatEncoderTests : EncoderTestsBase<float>
    {
        public FloatEncoderTests()
            : base(new FloatEncoder())
        {
        }

        [Theory]
        [InlineData(0, "0X2080000000")]
        [InlineData(1, "0X20BF800000")]
        [InlineData(-1, "0X20407FFFFF")]
        [InlineData(float.NaN, "0X20003FFFFF")]
        [InlineData(12.11F, "0X20C141C28F")]
        [InlineData(float.PositiveInfinity, "0X20FF800000")]
        [InlineData(float.NegativeInfinity, "0X20007FFFFF")]
        public void CanEncodeFloat(float value, string expected)
            => EncodeAndVerify(value, expected);
    }
}
