namespace FoundationDbNet
{
    using System;
    using System.Threading.Tasks;
    using FoundationDbNet.Futures;
    using FoundationDbNet.Native;
    using FoundationDbNet.Native.SafeHandles;

    public sealed class FdbConnection : IDisposable
    {
        private readonly FdbClusterHandle _cluster;

        private bool _disposed;

        internal FdbConnection(FdbClusterHandle cluster)
        {
            _cluster = cluster;
            _disposed = false;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _cluster.Dispose();

            _disposed = true;
        }

        public async Task<FdbDatabase> OpenDefaultDatabaseAsync()
        {
            var futureHandle = NativeMethods.fdb_cluster_create_database(
                _cluster,
                FdbConstants.DefaultDbName,
                FdbConstants.DefaultDbName.Length);

            try
            {
                using (var future = new FdbDatabaseFuture(futureHandle))
                {
                    futureHandle = null;

                    var db = await future.ToTask().ConfigureAwait(false);

                    return new FdbDatabase(db);
                }
            }
            finally
            {
                // Prevent leaking future handle in case of exception.
                futureHandle?.Dispose();
            }
        }
    }
}
