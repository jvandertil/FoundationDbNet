namespace FoundationDbNet.Layers.Tuple
{
    using System;
    using System.Collections.Generic;

    public class TupleBuilder
    {
        private readonly List<ReadOnlyMemory<byte>> _buffers;

        private readonly IEncoder<Guid> _guidEncoder = new GuidEncoder();

        public TupleBuilder()
        {
            _buffers = new List<ReadOnlyMemory<byte>>(8);
        }

        public TupleBuilder Add(Guid value)
        {
            _buffers.Add(_guidEncoder.Encode(value));

            return this;
        }

        internal int GetEncodedLength()
        {
            int sum = 0;

            foreach (var buffer in _buffers)
            {
                sum += buffer.Length;
            }

            return sum;
        }

        public ReadOnlySpan<byte> Pack()
        {
            Span<byte> result = new byte[GetEncodedLength()];
            Span<byte> destination = result;

            int offset = 0;
            foreach (var buffer in _buffers)
            {
                buffer.Span.CopyTo(destination);

                offset += buffer.Length;

                destination = result.Slice(offset);
            }

            return result;
        }
    }
}
