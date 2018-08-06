namespace FoundationDbNet.Native
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        [DllImport(NativeConstants.FdbDll)]
        internal static extern IntPtr fdb_get_error(FdbError error);
    }
}
