namespace FoundationDbNet.Layers.Tuple.Encoders
{
    using System;

    internal static class BooleanEncoder
    {
        private static readonly ReadOnlyMemory<byte> FalseValue = new byte[] { 0x26 };
        private static readonly ReadOnlyMemory<byte> TrueValue = new byte[] { 0x27 };

        public static ReadOnlyMemory<byte> Encode(bool value)
        {
            return value ? TrueValue : FalseValue;
        }
    }
}
