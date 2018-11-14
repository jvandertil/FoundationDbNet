namespace FoundationDbNet
{
    using System;
    using System.Threading.Tasks;

    public interface IFdbTransaction : IFdbReadTransaction
    {
        Task CommitAsync();

        void Set(ReadOnlySpan<byte> key, ReadOnlySpan<byte> value);
    }
}
