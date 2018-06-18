namespace FoundationDbNet.Native.SafeHandles
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a wrapper class for a FoundationDB transaction handle.
    /// </summary>
    public sealed class FdbTransactionHandle : SafeHandle
    {
        /// <inheritdoc cref="SafeHandle.IsInvalid"/>
        public override bool IsInvalid
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get => handle == IntPtr.Zero;
        }

        private FdbTransactionHandle()
            : base(IntPtr.Zero, true)
        {
        }

        /// <inheritdoc cref="ReleaseHandle"/>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            // Release the FDB handle.
            NativeMethods.fdb_transaction_destroy(handle);

            return true;
        }
    }
}
