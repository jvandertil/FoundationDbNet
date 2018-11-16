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

            if (value > 0)
            {
                return EncodePositive(value);
            }

            return EncodeNegative(value);
        }

        private ReadOnlySpan<byte> EncodePositive(long value)
        {
            int size = GetEncodedSizeInBytes((ulong)value);

            Span<byte> result = new byte[8 + 1];
            result[0] = (byte)(ZeroValueByte + size);

            var destination = result.Slice(1);

            MemoryMarshal.Write(destination, ref value);
            destination.Slice(0, size).Reverse();

            return result.Slice(0, size + 1);
        }

        private ReadOnlySpan<byte> EncodeNegative(long value)
        {
            int size = GetEncodedSizeInBytes((ulong)-value);

            Span<byte> result = new byte[8 + 1];
            result[0] = (byte)(ZeroValueByte - size);

            var destination = result.Slice(1);

            var onesComplementValue = ((long)SizeLimits[size - 1]) + value;

            MemoryMarshal.Write(destination, ref onesComplementValue);
            destination.Slice(0, size).Reverse();

            return result.Slice(0, size + 1);
        }

        private int GetEncodedSizeInBytes(ulong value)
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
