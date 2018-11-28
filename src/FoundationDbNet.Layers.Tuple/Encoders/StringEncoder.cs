namespace FoundationDbNet.Layers.Tuple.Encoders
{
    using System;
    using System.Buffers;
    using System.Text;

    internal static class StringEncoder
    {
        private const byte UnicodeStringMarkerByte = 0x02;
        private const byte TerminatingByte = 0x00;

        private static readonly Encoding Utf8Encoding = Encoding.UTF8;

        private static readonly ArrayPool<byte> ArrayPool = ArrayPool<byte>.Shared;
        private static readonly ReadOnlyMemory<byte> EmptyValue = new byte[] { UnicodeStringMarkerByte, TerminatingByte };

        public static ReadOnlyMemory<byte> Encode(string value)
        {
            if (value.Length == 0)
            {
                return EmptyValue;
            }

            int bytesWritten = 0;
            byte[] scratch = null;
            Span<byte> source = default;

            try
            {
                // Get large enough scratch buffer from array pool.
                int maxNumberOfBytes = Utf8Encoding.GetMaxByteCount(value.Length);
                scratch = ArrayPool.Rent(maxNumberOfBytes);

                bytesWritten = Utf8Encoding.GetBytes(value, 0, value.Length, scratch, 0);
                source = scratch.AsSpan(0, bytesWritten);

                int numberOfNullBytes = GetNumberOfNullBytes(source);

                // 2 marker bytes (start and end), the actual string, and null bytes are expanded to 2 bytes.
                int resultSize = 1 + bytesWritten + numberOfNullBytes + 1;

                var result = new byte[resultSize];

                // Place start and end markers.
                result[0] = UnicodeStringMarkerByte;
                result[result.Length - 1] = TerminatingByte;

                // Create slice to ease writing into result.
                var destination = result.AsSpan().Slice(1, result.Length - 2);

                if (numberOfNullBytes == 0)
                {
                    // CopyTo is faster, so if we don't need to do any 0x00 escaping, use that.
                    source.CopyTo(destination);
                }
                else
                {
                    int position = 0;

                    for (int i = 0; i < source.Length; ++i)
                    {
                        byte val = destination[position++] = source[i];

                        if (val == 0x00)
                        {
                            destination[position++] = 0xFF;
                        }
                    }
                }

                return result;
            }
            finally
            {
                if (scratch != null)
                {
                    source.Clear();

                    // If bytesWritten == 0 and we are here, then an exception occurred while encoding the string.
                    // In this case we can not know if anything was written to the buffer.
                    // To avoid leaking any data to the pool, let the pool clear it.
                    // If bytesWritten != 0 then source should have taken care of clearing only what was written.
                    ArrayPool.Return(scratch, bytesWritten == 0);
                }
            }
        }

        private static int GetNumberOfNullBytes(ReadOnlySpan<byte> value)
        {
            int nullBytesCount = 0;

            for (int i = 0; i < value.Length; ++i)
            {
                if (value[i] == 0x00)
                {
                    ++nullBytesCount;
                }
            }

            return nullBytesCount;
        }
    }
}
