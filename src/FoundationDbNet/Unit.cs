namespace FoundationDbNet
{
    using System;
    
    /// <summary>
    /// Represents a void type. 
    /// </summary>
    internal struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
    {
        public static readonly Unit Value = new Unit();

        public override int GetHashCode()
        {
            return 42;
        }

        public override string ToString()
        {
            return "()";
        }

        public int CompareTo(Unit other)
        {
            return 0;
        }

        public int CompareTo(object obj)
        {
            return 0;
        }

        public bool Equals(Unit other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Unit;
        }

        public static bool operator ==(Unit first, Unit second)
        {
            return true;
        }

        public static bool operator !=(Unit first, Unit second)
        {
            return false;
        }
    }
}
