namespace FoundationDbNet.Layers.Tuple
{
    using System;
    using System.Runtime.InteropServices;

    public class IntegerEncoder : IEncoder<long>
    {
        private const byte ZeroValueByte = 0x14;

        private static readonly byte[] ZeroValue = { ZeroValueByte };
        private static readonly ulong[] SizeLimits;

        static IntegerEncoder()
        {
            unchecked
            {
                // Populate the max value that can be stored in a number of bytes:
                // index 0 = max value in 1 byte
                // index 7 = max value in 8 bytes
                SizeLimits = new[]
                {
                    (1UL << 08) - 1,
                    (1UL << 16) - 1,
                    (1UL << 24) - 1,
                    (1UL << 32) - 1,
                    (1UL << 40) - 1,
                    (1UL << 48) - 1,
                    (1UL << 56) - 1,
                    (1UL << 64) - 2,
                };
            }
        }

        public ReadOnlySpan<byte> Encode(long value)
        {
            if (value == 0)
            {
                return ZeroValue;
            }

            int size = value > 0 ? NumberOfBytes((ulong)value) : NumberOfBytes((ulong)-value);
            byte markerByte = value > 0 ? (byte)(ZeroValueByte + size) : (byte)(ZeroValueByte - size);

            // Take ones complement for negative values.
            long valueToWrite = value > 0 ? value : ((long)SizeLimits[size - 1]) + value;

            Span<byte> result = new byte[8 + 1];

            result[0] = markerByte;
            WriteValueBigEndian(result.Slice(1), size, valueToWrite);

            return result.Slice(0, size + 1);
        }

        private static void WriteValueBigEndian(Span<byte> buffer, int byteSize, long value)
        {
            MemoryMarshal.Write(buffer, ref value);
            buffer.Slice(0, byteSize).Reverse();
        }

        private static int NumberOfBytes(ulong value)
        {
            int size = SizeLimits.Length;
            for (int i = 0; i < SizeLimits.Length; ++i)
            {
                if (value <= SizeLimits[i])
                {
                    size = i + 1;
                    break;
                }
            }

            return size;
        }
    }
}
