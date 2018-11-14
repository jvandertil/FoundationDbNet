namespace FoundationDbNet.Native.Futures
{
    using System.Runtime.InteropServices;
    using FoundationDbNet.Native.SafeHandles;

    internal sealed class FdbDatabaseFuture : FdbFuture<FdbDatabaseHandle>
    {
        public FdbDatabaseFuture(FdbFutureHandle futureHandle)
            : base(futureHandle)
        {
        }

        protected override FdbDatabaseHandle GetResult()
        {
            fdb_future_get_database(Handle, out FdbDatabaseHandle database)
                .EnsureSuccess();

            return database;
        }

        [DllImport(FdbConstants.FdbDll)]
        private static extern FdbError fdb_future_get_database(FdbFutureHandle future, out FdbDatabaseHandle database);
    }
}
