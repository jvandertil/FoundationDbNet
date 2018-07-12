
namespace FoundationDbNet
{
    using System;
    using FoundationDbNet.Native;
    using FoundationDbNet.Native.SafeHandles;

    public sealed class FdbDatabase : IDisposable
    {
        private readonly FdbDatabaseHandle _database;
        private bool _disposed;

        internal FdbDatabase(FdbDatabaseHandle database)
        {
            _database = database;
            _disposed = false;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _database.Dispose();

            _disposed = true;
        }

        public FdbTransaction BeginTransaction()
        {
            NativeMethods.fdb_database_create_transaction(_database, out var localTx)
                .EnsureSuccess();

            try
            {
                var fdbTransaction = new FdbTransaction(localTx);

                localTx = null;

                return fdbTransaction;
            }
            finally
            {
                // Prevent leaking the handle.
                localTx?.Dispose();
            }
        }
    }
}
