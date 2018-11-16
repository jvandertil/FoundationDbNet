namespace FoundationDbNet.Tests.Features.Layers.Tuple
{
    using System;
    using FoundationDbNet.Layers.Tuple;
    using FoundationDbNet.Tests.Framework.Layers.Tuple;
    using Xunit;

    public class ByteMemoryEncoderTests : EncoderTestsBase<ReadOnlyMemory<byte>>
    {
        public ByteMemoryEncoderTests()
            : base(new ByteMemoryEncoder())
        {
        }

        [Theory]
        [InlineData(new byte[0], "0x0100")]
        [InlineData(new byte[] { 0x02, 0x03, 0x41, 0x11 }, "0x010203411100")]
        [InlineData(new byte[] { 0x02, 0x03, 0x00, 0x11 }, "0x01020300FF1100")]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, "0x0100FF00FF00FF00FF00")]
        public void CanEncodeByteArray(byte[] data, string expected)
            => EncodeAndVerify(data, expected);
    }
}
