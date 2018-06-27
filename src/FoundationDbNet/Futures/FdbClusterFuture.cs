namespace FoundationDbNet.Futures
{
    using FoundationDbNet.Native;
    using FoundationDbNet.Native.SafeHandles;

    internal sealed class FdbClusterFuture : FdbFuture<FdbClusterHandle>
    {
        public FdbClusterFuture(FdbFutureHandle futureHandle)
            : base(futureHandle)
        {
        }

        protected override FdbClusterHandle GetResult()
        {
            NativeMethods.fdb_future_get_cluster(Handle, out var cluster)
                .EnsureSuccess();

            return cluster;
        }
    }
}
