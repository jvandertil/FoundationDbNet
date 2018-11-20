namespace FoundationDbNet.Layers.Tuple
{
    using System;
    using System.Runtime.InteropServices;

    internal class DoubleEncoder
    {
        private const byte DoubleMarkerByte = 0x21;

        public static ReadOnlyMemory<byte> Encode(double value)
        {
            var result = new byte[8 + 1];
            result[0] = DoubleMarkerByte;

            var destination = result.AsSpan().Slice(1);

            WriteBigEndian(destination, ref value);
            ApplyCustomTransformation(destination);

            return result;
        }

        private static void WriteBigEndian(Span<byte> destination, ref double value)
        {
            MemoryMarshal.Write(destination, ref value);

            if (BitConverter.IsLittleEndian)
            {
                destination.Reverse();
            }
        }

        private static void ApplyCustomTransformation(Span<byte> buffer)
        {
            if ((buffer[0] & 0x80) == 0)
            {
                // Positive number, flip the sign bit.
                buffer[0] ^= 0x80;
            }
            else
            {
                // Negative number, flip all bits.
                for (int i = 0; i < buffer.Length; ++i)
                {
                    buffer[i] ^= 0xFF;
                }
            }
        }
    }
}
