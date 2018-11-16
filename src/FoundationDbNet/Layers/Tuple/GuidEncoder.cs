namespace FoundationDbNet.Layers.Tuple
{
    using System;

    public class GuidEncoder : IEncoder<Guid>
    {
        private const byte GuidMarkerByte = 0x30;

        public ReadOnlySpan<byte> Encode(Guid value)
        {
            Span<byte> bytes = value.ToByteArray().AsSpan();
            Span<byte> result = new byte[1 + 16];

            result[0] = GuidMarkerByte;

            // Guid byte values need some reordering back to big endian.
            // Little endian DWORD
            result[1] = bytes[3];
            result[2] = bytes[2];
            result[3] = bytes[1];
            result[4] = bytes[0];

            // Little endian WORD
            result[5] = bytes[5];
            result[6] = bytes[4];

            // Little endian WORD
            result[7] = bytes[7];
            result[8] = bytes[6];

            var source = bytes.Slice(8);

            source.CopyTo(result.Slice(9));

            return result;
        }
    }
}
