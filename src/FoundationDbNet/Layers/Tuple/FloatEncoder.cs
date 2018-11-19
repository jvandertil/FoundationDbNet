namespace FoundationDbNet.Layers.Tuple
{
    using System;
    using System.Runtime.InteropServices;

    public class FloatEncoder : IEncoder<float>
    {
        private const byte FloatMarkerByte = 0x20;

        public ReadOnlyMemory<byte> Encode(float value)
        {
            var result = new byte[4 + 1];
            result[0] = FloatMarkerByte;

            var destination = result.AsSpan().Slice(1);

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
