namespace FoundationDbNet.Layers.Tuple
{
    using System;
    using System.Collections.Generic;

    public sealed class FdbTuple
    {
        private static readonly ReadOnlyMemory<byte> NullValue = new byte[] { 0x00 };

        private delegate ReadOnlyMemory<byte> ObjectEncoder<T>(T value) where T : class;
        private delegate ReadOnlyMemory<byte> ValueTypeEncoder<T>(T value) where T : struct;

        private readonly List<ReadOnlyMemory<byte>> _buffers;

        public FdbTuple()
        {
            _buffers = new List<ReadOnlyMemory<byte>>(4);
        }

        public FdbTuple Add(bool value)
            => AddEncoded(value, x => BooleanEncoder.Encode(x));

        public FdbTuple Add(Guid value)
            => AddEncoded(value, x => GuidEncoder.Encode(x));

        public FdbTuple Add(long value)
            => AddEncoded(value, x => IntegerEncoder.Encode(x));

        public FdbTuple Add(double value)
            => AddEncoded(value, x => DoubleEncoder.Encode(x));

        public FdbTuple Add(ReadOnlyMemory<byte> value)
            => AddEncoded(value, x => ByteMemoryEncoder.Encode(x));

        public FdbTuple Add(float value)
            => AddEncoded(value, x => FloatEncoder.Encode(x));

        public FdbTuple Add(string value)
            => AddEncoded(value, x => StringEncoder.Encode(x));

        public ReadOnlySpan<byte> Pack()
        {
            if (_buffers.Count == 0)
            {
                return Array.Empty<byte>();
            }

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

        private FdbTuple AddEncoded<T>(T value, ObjectEncoder<T> encoder) where T : class
        {
            if (value == null)
            {
                _buffers.Add(NullValue);
            }
            else
            {
                _buffers.Add(encoder(value));
            }

            return this;
        }

        private FdbTuple AddEncoded<T>(T value, ValueTypeEncoder<T> encoder) where T : struct
        {
            _buffers.Add(encoder(value));

            return this;
        }

        private int GetEncodedLength()
        {
            int sum = 0;

            for (int i = 0; i < _buffers.Count; ++i)
            {
                sum += _buffers[i].Length;
            }

            return sum;
        }
    }
}
