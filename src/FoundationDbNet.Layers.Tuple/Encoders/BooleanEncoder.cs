namespace FoundationDbNet.Layers.Tuple
{
    using System;

    internal class BooleanEncoder
    {
        private static readonly byte[] FalseValue = { 0x26 };
        private static readonly byte[] TrueValue = { 0x27 };

        public static ReadOnlyMemory<byte> Encode(bool value)
        {
            return value ? TrueValue : FalseValue;
        }
    }
}
