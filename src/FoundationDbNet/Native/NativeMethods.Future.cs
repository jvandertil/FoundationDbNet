namespace FoundationDbNet.Native
{
    using System;
    using System.Runtime.InteropServices;
    using FoundationDbNet.Native.SafeHandles;

    internal static partial class NativeMethods
    {
        [DllImport(FdbDll)]
        public static extern FdbError fdb_future_set_callback(
            FdbFutureHandle future,
            [MarshalAs(UnmanagedType.FunctionPtr)] FdbCallback callbackMethod,
            IntPtr callbackParameter);

        [DllImport(FdbDll)]
        public static extern FdbError fdb_future_get_error(FdbFutureHandle future);

        [DllImport(FdbDll)]
        public static extern FdbError fdb_future_get_cluster(FdbFutureHandle future, out FdbClusterHandle cluster);
    }
}
