using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace FoundationDbNet.Layers.Tuple.Decoders
{
    internal static class GuidDecoder
    {
        private static readonly ArrayPool<byte> arrayPool = ArrayPool<byte>.Shared;

        public static Guid Decode(ReadOnlyMemory<byte> slice)
        {
            var scratchArray = arrayPool.Rent(16);
            var scratch = scratchArray.AsSpan(0, 16);

            try
            {
                slice.Slice(1).Span.CopyTo(scratch);

                scratch.Slice(0, 4).Reverse();
                scratch.Slice(4, 2).Reverse();
                scratch.Slice(6, 2).Reverse();

                return MemoryMarshal.Read<Guid>(scratch);
            }
            finally
            {
                scratch.Clear();
                arrayPool.Return(scratchArray);
            }
        }
    }
}
