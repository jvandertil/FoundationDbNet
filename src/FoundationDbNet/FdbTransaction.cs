namespace FoundationDbNet
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using FoundationDbNet.Futures;
    using FoundationDbNet.Native;
    using FoundationDbNet.Native.SafeHandles;

    public sealed class FdbTransaction : IDisposable
    {
        private readonly FdbTransactionHandle _transaction;
        private bool _disposed;

        internal FdbTransaction(FdbTransactionHandle transaction)
        {
            _transaction = transaction;
            _disposed = false;
        }

        public async Task CommitAsync()
        {
            var txHandle = NativeMethods.fdb_transaction_commit(_transaction);
            try
            {
                var txFuture = new FdbCommitFuture(txHandle);

                txHandle = null;

                await txFuture.ToTask().ConfigureAwait(false);
            }
            finally
            {
                txHandle?.Dispose();
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _transaction.Dispose();

            _disposed = true;
        }

        public void Set(ReadOnlySpan<byte> key, ReadOnlySpan<byte> value)
        {
            unsafe
            {
                fixed (byte* keyPtr = key)
                fixed (byte* valuePtr = value)
                {
                    NativeMethods.fdb_transaction_set(
                        _transaction,
                        Unsafe.AsRef<byte>(keyPtr),
                        key.Length,
                        Unsafe.AsRef<byte>(valuePtr),
                        value.Length);
                }
            }
        }

        public Task<byte[]> Get(ReadOnlySpan<byte> key)
        {
            FdbFutureHandle handle = null;
            FdbGetValueFuture future = null;

            try
            {
                unsafe
                {
                    fixed (byte* keyPtr = key)
                    {
                        handle = NativeMethods.fdb_transaction_get(
                            _transaction,
                            Unsafe.AsRef<byte>(keyPtr),
                            key.Length,
                            false);
                    }
                }

                future = new FdbGetValueFuture(handle);
                handle = null;

                var task = future.ToTask();

                future = null;

                return task;
            }
            finally
            {
                handle?.Dispose();
                future?.Dispose();
            }
        }

        public void Clear(ReadOnlySpan<byte> key)
        {
            unsafe
            {
                fixed (byte* keyPtr = key)
                {
                    NativeMethods.fdb_transaction_clear(
                        _transaction,
                        Unsafe.AsRef<byte>(keyPtr),
                        key.Length);
                }
            }
        }
    }
}
