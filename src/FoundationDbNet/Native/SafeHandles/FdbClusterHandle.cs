namespace FoundationDbNet.Native.SafeHandles
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a wrapper class for a FoundationDB cluster handle.
    /// </summary>
    public sealed class FdbClusterHandle : SafeHandle
    {
        /// <inheritdoc cref="SafeHandle.IsInvalid"/>
        public override bool IsInvalid
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get => handle == IntPtr.Zero;
        }

        private FdbClusterHandle()
            : base(IntPtr.Zero, true)
        {
        }

        /// <inheritdoc cref="ReleaseHandle"/>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            // Release the FDB handle.
            NativeMethods.fdb_cluster_destroy(handle);

            return true;
        }
    }
}
