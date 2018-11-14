namespace FoundationDbNet
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using FoundationDbNet.Logging;
    using FoundationDbNet.Native.Futures;
    using FoundationDbNet.Native.SafeHandles;

    internal sealed class FdbTransaction : IFdbTransaction
    {
        private static readonly ILog Logger = LogProvider.For<FdbTransaction>();

        private readonly FdbTransactionHandle _handle;
        private readonly bool _snapshot;
        private bool _disposed;

        internal FdbTransaction(FdbTransactionHandle transaction, bool snapshot)
        {
            _handle = transaction ?? throw new ArgumentNullException(nameof(transaction));

            _snapshot = snapshot;
            _disposed = false;
        }

        public Task CommitAsync()
        {
            return fdb_transaction_commit(_handle)
                    .SafeMap(h => new FdbVoidFuture(h))
                    .ToTask();
        }

        public void Set(ReadOnlySpan<byte> key, ReadOnlySpan<byte> value)
        {
            unsafe
            {
                fixed (byte* keyPtr = key)
                fixed (byte* valuePtr = value)
                {
                    fdb_transaction_set(
                        _handle,
                        keyPtr, key.Length,
                        valuePtr, value.Length);
                }
            }
        }

        public Task<FdbValue> GetAsync(ReadOnlySpan<byte> key)
        {
            FdbFutureHandle future;

            unsafe
            {
                fixed (byte* keyPtr = key)
                {
                    future = fdb_transaction_get(_handle, keyPtr, key.Length, _snapshot);
                }
            }

            return future
                    .SafeMap(x => new FdbValueFuture(x))
                    .ToTask();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _handle.Dispose();

            _disposed = true;
        }

        [DllImport(FdbConstants.FdbDll)]
        private static extern FdbFutureHandle fdb_transaction_commit(FdbTransactionHandle transaction);

        [DllImport(FdbConstants.FdbDll)]
        private static unsafe extern void fdb_transaction_set(
            FdbTransactionHandle transaction,
            byte* key, int keyLen,
            byte* value, int valueLen);

        [DllImport(FdbConstants.FdbDll)]
        private static unsafe extern FdbFutureHandle fdb_transaction_get(
            FdbTransactionHandle transaction,
            byte* key, int keyLen,
            [MarshalAs(UnmanagedType.Bool)] bool snapshot);
    }
}
