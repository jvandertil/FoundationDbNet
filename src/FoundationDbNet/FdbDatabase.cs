namespace FoundationDbNet
{
    using System;
    using System.Runtime.InteropServices;
    using FoundationDbNet.Logging;
    using FoundationDbNet.Native.SafeHandles;

    internal sealed class FdbDatabase : IFdbDatabase
    {
        private static readonly ILog Logger = LogProvider.For<FdbDatabase>();

        private readonly FdbDatabaseHandle _database;
        private bool _disposed;

        internal FdbDatabase(FdbDatabaseHandle database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _disposed = false;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _database.Dispose();

            _disposed = true;
        }

        public IFdbTransaction BeginTransaction()
        {
            Logger.Debug("Starting FoundationDB transaction.");

            fdb_database_create_transaction(_database, out var txHandle)
                .EnsureSuccess();

            return txHandle.SafeMap(h => new FdbTransaction(h, false));
        }

        public IFdbReadTransaction BeginSnapshotTransaction()
        {
            Logger.Debug("Starting FoundationDB transaction in snapshot mode.");

            fdb_database_create_transaction(_database, out var txHandle)
                .EnsureSuccess();

            return txHandle.SafeMap(h => new FdbTransaction(h, true));
        }

        [DllImport(FdbConstants.FdbDll)]
        private static extern FdbError fdb_database_create_transaction(FdbDatabaseHandle database, out FdbTransactionHandle transaction);
    }
}
