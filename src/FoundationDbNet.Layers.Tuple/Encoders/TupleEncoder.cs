namespace FoundationDbNet.Layers.Tuple.Encoders
{
    using System;

    internal class TupleEncoder
    {
        private const byte TupleMarker = 0x05;
        private const byte TerminatingByte = 0x00;

        private static readonly ReadOnlyMemory<byte> EscapedNullValue = new byte[] { 0x00, 0xFF };

        public static ReadOnlyMemory<byte> Encode(FdbTuple tuple)
        {
            int escapedSize = 0;

            var buffers = tuple.GetBuffersInternal();

            for (int i = 0; i < buffers.Count; ++i)
            {
                if (IsNullValue(buffers[i]))
                {
                    escapedSize += EscapedNullValue.Length;
                }
                else
                {
                    escapedSize += buffers[i].Length;
                }
            }

            byte[] result = new byte[escapedSize + 2];
            Memory<byte> destination = result.AsMemory().Slice(1);

            for (int i = 0; i < buffers.Count; ++i)
            {
                var buffer = buffers[i];

                if (IsNullValue(buffer))
                {
                    buffer = EscapedNullValue;
                }

                buffer.CopyTo(destination);
                destination = destination.Slice(buffer.Length);
            }

            result[0] = TupleMarker;
            result[result.Length - 1] = TerminatingByte;

            return result;
        }

        private static bool IsNullValue(ReadOnlyMemory<byte> buffer)
        {
            return buffer.Length == 1
                   && buffer.Span[0] == 0x00;
        }
    }
}
