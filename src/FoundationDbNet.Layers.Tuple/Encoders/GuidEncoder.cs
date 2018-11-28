namespace FoundationDbNet.Layers.Tuple.Encoders
{
    using System;

    internal static class GuidEncoder
    {
        private const byte GuidMarkerByte = 0x30;

        public static ReadOnlyMemory<byte> Encode(Guid value)
        {
            Span<byte> source = value.ToByteArray().AsSpan();
            var result = new byte[1 + 16];

            result[0] = GuidMarkerByte;

            var destination = result.AsSpan().Slice(1);

            if (BitConverter.IsLittleEndian)
            {
                // Guid byte values need some reordering back to big endian.
                source.Slice(0, 4).Reverse();
                source.Slice(4, 2).Reverse();
                source.Slice(6, 2).Reverse();
            }

            source.CopyTo(destination);

            return result;
        }
    }
}
