namespace FoundationDbNet.Futures
{
    using FoundationDbNet.Native;
    using FoundationDbNet.Native.SafeHandles;

    internal sealed class FdbDatabaseFuture : FdbFuture<FdbDatabaseHandle>
    {
        public FdbDatabaseFuture(FdbFutureHandle futureHandle)
            : base(futureHandle)
        {
        }

        protected override FdbDatabaseHandle GetResult()
        {
            NativeMethods.fdb_future_get_database(Handle, out var database)
                .EnsureSuccess();

            return database;
        }
    }
}
