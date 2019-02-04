namespace FoundationDbNet
{
    using System;
    using System.Buffers;

    public class FdbValue : IEquatable<FdbValue>, IDisposable
    {
        public static readonly FdbValue NonExistent = new FdbValue(false, ReadOnlyMemory<byte>.Empty);

        public static readonly FdbValue Empty = new FdbValue(true, ReadOnlyMemory<byte>.Empty);

        public bool IsPresent { get; }

        public IMemoryOwner<byte> Value { get; }

        public FdbValue(bool present, ReadOnlyMemory<byte> value)
        {
            IsPresent = present;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is FdbValue other)
            {
                return Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 27;

                hashCode = (13 * hashCode) + IsPresent.GetHashCode();
                hashCode = (13 * hashCode) + Value.GetHashCode();

                return hashCode;
            }
        }

        public bool Equals(FdbValue other)
        {
            return (IsPresent == other.IsPresent)
                   && (Value.Equals(other.Value));
        }

        public static bool operator ==(FdbValue left, FdbValue right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FdbValue left, FdbValue right)
        {
            return !(left == right);
        }
    }
}
