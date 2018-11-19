namespace FoundationDbNet.Layers.Tuple
{
    using System;

    public interface IEncoder<in T>
    {
        ReadOnlyMemory<byte> Encode(T value);
    }
}
