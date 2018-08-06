namespace FoundationDbNet.Native.Networking
{
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        [DllImport(NativeConstants.FdbDll)]
        public static extern FdbError fdb_select_api_version_impl(int runtimeVersion, int headerVersion);

        [DllImport(NativeConstants.FdbDll)]
        public static extern FdbError fdb_setup_network();

        [DllImport(NativeConstants.FdbDll)]
        public static extern FdbError fdb_run_network();

        [DllImport(NativeConstants.FdbDll)]
        public static extern FdbError fdb_stop_network();
    }
}
