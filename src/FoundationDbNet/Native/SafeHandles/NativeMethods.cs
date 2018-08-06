namespace FoundationDbNet.Native.SafeHandles
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    internal static class NativeMethods
    {
        [DllImport(NativeConstants.FdbDll)]
        [SuppressUnmanagedCodeSecurity]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static extern void fdb_database_destroy(IntPtr database);

        [DllImport(NativeConstants.FdbDll)]
        [SuppressUnmanagedCodeSecurity]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static extern void fdb_cluster_destroy(IntPtr cluster);

        [DllImport(NativeConstants.FdbDll)]
        [SuppressUnmanagedCodeSecurity]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static extern void fdb_transaction_destroy(IntPtr transaction);

        [DllImport(NativeConstants.FdbDll)]
        [SuppressUnmanagedCodeSecurity]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static extern void fdb_future_destroy(IntPtr future);
    }
}
