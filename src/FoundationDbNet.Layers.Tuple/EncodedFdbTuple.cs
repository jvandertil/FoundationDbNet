using System;
using System.Collections.Generic;
using System.Linq;
using FoundationDbNet.Layers.Tuple.Decoders;

namespace FoundationDbNet.Layers.Tuple
{
    public sealed class EncodedFdbTuple
    {
        private delegate object Decoder(ReadOnlyMemory<byte> slice);

        private static Dictionary<Type, Decoder> _decoders = new Dictionary<Type, Decoder>
        {
            [typeof(Guid)] = slice => GuidDecoder.Decode(slice),
            [typeof(long)] = slice => IntegerDecoder.Decode(slice),
        };

        private readonly ReadOnlyMemory<byte> _encoded;
        private readonly (Type type, ReadOnlyMemory<byte> contents)[] _slices;

        public EncodedFdbTuple(ReadOnlyMemory<byte> encodedTuple)
        {
            _encoded = encodedTuple;

            _slices = Parse().ToArray();
        }

        public T Get<T>(int index)
        {
            if ((uint)index >= _slices.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Index out of range [0, {_slices.Length})");
            }

            var slice = _slices[index];
            var argType = typeof(T);

            if (!slice.type.IsAssignableFrom(argType))
            {
                throw new InvalidOperationException($"Type mismatch, actual: {_slices[index].type}, expected: {argType}.");
            }

            return (T)_decoders[slice.type](slice.contents);
        }

        private IEnumerable<(Type, ReadOnlyMemory<byte>)> Parse()
        {
            int bytesRead = 0;
            var source = _encoded;

            while (BytesLeft())
            {
                var slice = ReadSlice(source);
                Consume(slice.Item2.Length);

                yield return slice;
            }

            bool BytesLeft() => bytesRead < _encoded.Length;
            void Consume(int bytes)
            {
                bytesRead += bytes;
                source = source.Slice(bytes);
            }
        }

        private (Type, ReadOnlyMemory<byte>) ReadSlice(ReadOnlyMemory<byte> slice)
        {
            switch (slice.Span[0])
            {
                case TypeMarkers.GuidMarker:
                    return (typeof(Guid), slice.Slice(0, 16 + 1));

                case TypeMarkers.IntegerZero:
                    return (typeof(long), slice.Slice(0, 0 + 1));

                case TypeMarkers.Integer8Byte:
                    return (typeof(long), slice.Slice(0, 8 + 1));
                default:
                    throw new InvalidOperationException();
            }
        }

        private int SeekNullTerminator(ReadOnlyMemory<byte> slice)
        {
            var span = slice.Span;

            for (int i = 0; i < slice.Length; ++i)
            {
                if (span[i] == 0x00)
                {
                    // if we reached the end, then we are done.
                    if (i == (span.Length - 1))
                    {
                        return i + 1;
                    }
                    else if (span[i + 1] != 0xFF)
                    {
                        // if this 0x00 byte is not followed by an 0xFF byte, it is the end.
                        return i;
                    }
                }
            }

            return slice.Length;
        }
    }

    internal static class TypeMarkers
    {
        public const byte ByteString = 0x01;
        public const byte Utf8String = 0x02;
        public const byte GuidMarker = 0x30;

        public const byte IntegerZero = 0x14;
        public const byte Integer8Byte = 0x1C;
    }
}
