namespace FoundationDbNet.Layers.Tuple
{
    using System;

    public class BooleanEncoder : IEncoder<bool>
    {
        private static readonly byte[] FalseValue = { 0x26 };
        private static readonly byte[] TrueValue = { 0x27 };

        public ReadOnlySpan<byte> Encode(bool value)
        {
            return value ? TrueValue : FalseValue;
        }
    }
}
