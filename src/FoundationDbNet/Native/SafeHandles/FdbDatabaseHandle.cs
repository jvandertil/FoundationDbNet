namespace FoundationDbNet.Native.SafeHandles
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a wrapper class for a FoundationDB database handle.
    /// </summary>
    public sealed class FdbDatabaseHandle : SafeHandle
    {
        /// <inheritdoc cref="SafeHandle.IsInvalid"/>
        public override bool IsInvalid
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get => handle == IntPtr.Zero;
        }

        private FdbDatabaseHandle()
            : base(IntPtr.Zero, true)
        {
        }

        /// <inheritdoc cref="ReleaseHandle"/>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            // Release the FDB handle.
            NativeMethods.fdb_database_destroy(handle);

            return true;
        }
    }
}
