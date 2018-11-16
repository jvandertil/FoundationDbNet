namespace FoundationDbNet.Layers.Tuple
{
    using System;

    public interface IEncoder<in T>
    {
        ReadOnlySpan<byte> Encode(T value);
    }
}
