namespace FoundationDbNet.Native
{
    using System;
    using System.Runtime.InteropServices;
    using FoundationDbNet.Native.SafeHandles;

    internal static partial class NativeMethods
    {
        public const string FdbDll = "libfdb_c";

        [DllImport(FdbDll, CharSet = CharSet.Ansi)]
        public static extern IntPtr fdb_get_error(FdbError error);

        [DllImport(FdbDll)]
        public static extern FdbError fdb_select_api_version_impl(int runtimeVersion, int headerVersion);

        [DllImport(FdbDll)]
        public static extern FdbError fdb_setup_network();

        [DllImport(FdbDll)]
        public static extern FdbError fdb_run_network();

        [DllImport(FdbDll)]
        public static extern FdbError fdb_stop_network();

        [DllImport(FdbDll)]
        public static extern int fdb_get_max_api_version();

        [DllImport(FdbDll)]
        public static extern FdbError fdb_database_create_transaction(FdbDatabaseHandle database, out FdbTransactionHandle transaction);

        [DllImport(FdbDll)]
        public static extern FdbFutureHandle fdb_transaction_commit(FdbTransactionHandle transaction);

        //[DllImport(FdbDll)]
        //public static extern FdbError fdb_cluster_set_option(FdbClusterHandle cluster, int option, byte[] value, int len);

        [DllImport(FdbDll)]
        public static extern void fdb_future_cancel(FdbFutureHandle future);

        [DllImport(FdbDll)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool fdb_future_is_ready(FdbFutureHandle future);

        [DllImport(FdbDll)]
        public static extern FdbError fdb_future_block_until_ready(FdbFutureHandle future);

        [DllImport(FdbDll)]
        public static extern FdbError fdb_future_get_version(FdbFutureHandle future, out long version);

        [DllImport(FdbDll)]
        public static extern FdbError fdb_future_get_key(FdbFutureHandle future, out IntPtr keyPtr, out int len);

        [DllImport(FdbDll)]
        public static extern void fdb_future_release_memory(FdbFutureHandle future);
    }
}
