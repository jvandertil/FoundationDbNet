namespace FoundationDbNet.Native
{
    using System.Runtime.InteropServices;
    using FoundationDbNet.Native.SafeHandles;

    internal static partial class NativeMethods
    {
        [DllImport(FdbDll, CharSet = CharSet.Unicode)]
        public static extern FdbFutureHandle fdb_create_cluster(string clusterFilePath);

        [DllImport(FdbDll)]
        public static extern FdbFutureHandle fdb_cluster_create_database(FdbClusterHandle cluster, byte[] dbName, int len);
    }
}
