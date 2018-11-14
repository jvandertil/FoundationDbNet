namespace FoundationDbNet.Native.Futures
{
    using System.Runtime.InteropServices;
    using FoundationDbNet.Native.SafeHandles;

    internal sealed class FdbClusterFuture : FdbFuture<FdbClusterHandle>
    {
        public FdbClusterFuture(FdbFutureHandle futureHandle)
            : base(futureHandle)
        {
        }

        protected override FdbClusterHandle GetResult()
        {
            fdb_future_get_cluster(Handle, out FdbClusterHandle cluster)
                .EnsureSuccess();

            return cluster;
        }

        [DllImport(FdbConstants.FdbDll)]
        private static extern FdbError fdb_future_get_cluster(FdbFutureHandle future, out FdbClusterHandle cluster);
    }
}
