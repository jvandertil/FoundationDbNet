namespace FoundationDbNet.Tests.Framework.Layers.Tuple
{
    using FoundationDbNet.Layers.Tuple;
    using NFluent;

    public abstract class EncoderTestsBase<T>
    {
        protected readonly IEncoder<T> Encoder;

        protected EncoderTestsBase(IEncoder<T> encoder)
        {
            Encoder = encoder;
        }

        protected void EncodeAndVerify(T value, ByteString expected)
            => EncodeAndVerify(value, expected.ToArray());

        protected void EncodeAndVerify(T value, byte[] expected)
        {
            var result = Encoder.Encode(value).ToArray();

            Check.That(result).IsEqualTo(expected);
        }
    }
}
