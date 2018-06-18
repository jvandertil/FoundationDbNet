namespace FoundationDbNet.Native
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class NativeMethods
    {
        public const string FdbDll = "libfdb_c";

        [DllImport(FdbDll, CharSet = CharSet.Ansi)]
        public static extern IntPtr fdb_get_error(FdbError error);
    }
}
