namespace FoundationDbNet.Native
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using FoundationDbNet.Native.SafeHandles;

    internal static partial class NativeMethods
    {
        [DllImport(FdbDll)]
        public static extern void fdb_transaction_set(
            FdbTransactionHandle transaction,
            in byte key,
            int keyLen,
            in byte value,
            int valueLen);

        [DllImport(FdbDll)]
        public static extern FdbFutureHandle fdb_transaction_get(
            FdbTransactionHandle transaction,
            in byte key,
            int keyLen,
            [MarshalAs(UnmanagedType.Bool)] bool snapshot);

        [DllImport(FdbDll)]
        public static extern void fdb_transaction_clear(
            FdbTransactionHandle transaction,
            in byte key,
            int keyLen);
    }
}
