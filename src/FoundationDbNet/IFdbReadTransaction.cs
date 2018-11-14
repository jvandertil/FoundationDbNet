namespace FoundationDbNet
{
    using System;
    using System.Threading.Tasks;

    public interface IFdbReadTransaction : IDisposable
    {
        Task<FdbValue> GetAsync(ReadOnlySpan<byte> key);
    }
}
