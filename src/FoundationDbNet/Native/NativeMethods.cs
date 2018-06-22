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
    }
}
