namespace FoundationDbNet.Tests.Framework.Layers.Tuple
{
    using System;

    public class ByteString
    {
        private readonly string _value;

        public ByteString(string value)
        {
            if (!value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Value should start with 0x", nameof(value));
            }

            _value = value;
        }

        public byte[] ToArray()
        {
            return HexToBytes(_value.AsSpan().Slice(2));
        }

        public override string ToString()
        {
            return _value;
        }

        // Taken from: https://stackoverflow.com/a/3974535
        // Changed to take a ReadOnlySpan<char> instead of a string.
        private static byte[] HexToBytes(ReadOnlySpan<char> str)
        {
            if (str.Length == 0 || str.Length % 2 != 0)
            {
                return new byte[0];
            }

            byte[] buffer = new byte[str.Length / 2];
            char c;
            for (int bx = 0, sx = 0; bx < buffer.Length; ++bx, ++sx)
            {
                // Convert first half of byte
                c = str[sx];
                buffer[bx] = (byte)((c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0')) << 4);

                // Convert second half of byte
                c = str[++sx];
                buffer[bx] |= (byte)(c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0'));
            }

            return buffer;
        }

        public static implicit operator ByteString(string value)
        {
            return new ByteString(value);
        }
    }
}
