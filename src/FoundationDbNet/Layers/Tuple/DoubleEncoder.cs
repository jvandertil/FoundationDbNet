using System;
using System.Runtime.InteropServices;

namespace FoundationDbNet.Layers.Tuple
{
    public class DoubleEncoder : IEncoder<double>
    {
        private const byte DoubleMarkerByte = 0x21;

        public ReadOnlySpan<byte> Encode(double value)
        {
            Span<byte> result = new byte[8 + 1];
            result[0] = DoubleMarkerByte;

            var destination = result.Slice(1);

            // Custom transformation
            MemoryMarshal.Write(destination, ref value);
            destination.Reverse();

            ApplyCustomTransformation(destination);

            return result;
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