namespace FoundationDbNet
{
    using System;
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
    }
}
