namespace FoundationDbNet.Tests.Features.Layers.Tuple
{
    using System;
    using System.Collections.Generic;
    using FoundationDbNet.Layers.Tuple;
    using FoundationDbNet.Tests.Framework.Layers.Tuple;
    using Xunit;

    public class GuidEncoderTests : EncoderTestsBase<Guid>
    {
        public GuidEncoderTests()
            : base(new GuidEncoder())
        {
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void CanEncodeGuid(Guid guid, string expected)
            => EncodeAndVerify(guid, expected);

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { Guid.Parse("00010203-0405-0607-0809-0a0b0c0d0e0f"), "0x30000102030405060708090a0b0c0d0e0f" };

            var value = Guid.NewGuid();

            yield return new object[] { value, "0x30" + value.ToString("N") };
        }
    }
}
