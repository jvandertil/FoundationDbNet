namespace FoundationDbNet
{
    using System;

    public interface IFdbDatabase : IDisposable
    {
        IFdbTransaction BeginTransaction();

        IFdbReadTransaction BeginSnapshotTransaction();
    }
}
