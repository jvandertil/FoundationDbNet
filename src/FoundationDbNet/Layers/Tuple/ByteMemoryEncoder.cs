﻿namespace FoundationDbNet.Layers.Tuple
{
    using System;

    public class ByteMemoryEncoder : IEncoder<ReadOnlyMemory<byte>>
    {
        private const byte ByteStringMarkerByte = 0x01;
        private const byte TerminatingByte = 0x00;

        private static readonly byte[] EmptyValue = { ByteStringMarkerByte, TerminatingByte };

        public ReadOnlySpan<byte> Encode(ReadOnlyMemory<byte> value)
        {
            if (value.IsEmpty)
            {
                return EmptyValue;
            }

            var source = value.Span;

            int numberOfNullBytes = GetNumberOfNullBytes(source);

            byte[] result = new byte[1 + value.Length + numberOfNullBytes + 1];
            var destination = result.AsSpan().Slice(1, result.Length - 2);

            if (numberOfNullBytes == 0)
            {
                source.CopyTo(destination);
            }
            else
            {
                int position = 0;

                for(int i = 0; i < source.Length; ++i)
                {
                    byte val = destination[position++] = source[i];

                    if(val == 0x00)
                    {
                        destination[position++] = 0xFF;
                    }
                }
            }

            result[0] = ByteStringMarkerByte;
            result[result.Length - 1] = TerminatingByte;

            return result;
        }

        private static int GetNumberOfNullBytes(ReadOnlySpan<byte> value)
        {
            int count = 0;

            for (int i = 0; i < value.Length; ++i)
            {
                if (value[i] == 0x00)
                {
                    ++count;
                }
            }

            return count;
        }
    }
}