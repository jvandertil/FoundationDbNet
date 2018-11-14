namespace FoundationDbNet
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using FoundationDbNet.Native.Futures;
    using FoundationDbNet.Native.SafeHandles;

    internal sealed class FdbConnection : IFdbConnection
    {
        private const string DefaultDatabaseName = "DB";

        private readonly FdbClusterHandle _cluster;

        private bool _disposed;

        internal FdbConnection(FdbClusterHandle cluster)
        {
            _cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));

            _disposed = false;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _cluster.Dispose();

            _disposed = true;
        }

        public Task<IFdbDatabase> OpenDefaultDatabaseAsync()
            => OpenDatabaseAsync(DefaultDatabaseName);

        private Task<IFdbDatabase> OpenDatabaseAsync(string dbName)
        {
            if (dbName == null)
            {
                throw new ArgumentNullException(nameof(dbName));
            }

            var name = Encoding.ASCII.GetBytes(dbName);

            var result =
                fdb_cluster_create_database(_cluster, name, name.Length)
                    .SafeMap(h => new FdbDatabaseFuture(h))
                    .ToTask()
                    .ContinueWith<IFdbDatabase>(
                        future => future.Result.SafeMap(h => new FdbDatabase(h)),
                        TaskContinuationOptions.OnlyOnRanToCompletion);

            return result;
        }

        [DllImport(FdbConstants.FdbDll)]
        private static extern FdbFutureHandle fdb_cluster_create_database(FdbClusterHandle cluster, byte[] dbName, int len);
    }
}
