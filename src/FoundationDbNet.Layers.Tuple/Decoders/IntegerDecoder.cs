using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace FoundationDbNet.Layers.Tuple.Decoders
{
    internal static class IntegerDecoder
    {
        private static readonly ArrayPool<byte> arrayPool = ArrayPool<byte>.Shared;

        public static long Decode(ReadOnlyMemory<byte> slice)
        {
            var span = slice.Span;

            byte marker = span[0];

            if(marker == 0x14)
            {
                return 0;
            }

            if(marker > 0x14)
            {
                var scratchArray = arrayPool.Rent(8);
                var scratch = scratchArray.AsSpan().Slice(0, 8);
                try
                {
                    span.Slice(1).CopyTo(scratch);
                    scratch.Reverse();

                    return MemoryMarshal.Read<long>(scratch);
                }
                finally
                {
                    scratch.Clear();
                    arrayPool.Return(scratchArray);
                }
            }

            if(marker < 0x14)
            {
                return -1;
            }

            throw new InvalidOperationException("Not a long");
        }
    }
}
