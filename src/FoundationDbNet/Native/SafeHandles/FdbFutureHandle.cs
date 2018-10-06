namespace FoundationDbNet.Native.SafeHandles
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    /// <summary>
    /// Represents a wrapper class for a FoundationDB future handle.
    /// </summary>
    /// <remarks>
    /// A FoundationDB future represents an asynchronous operation in unmanaged code.
    /// </remarks>
    internal sealed class FdbFutureHandle : SafeHandle
    {
        /// <inheritdoc cref="SafeHandle.IsInvalid"/>
        public override bool IsInvalid
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get => handle == IntPtr.Zero;
        }

        private FdbFutureHandle()
            : base(IntPtr.Zero, true)
        {
        }

        /// <inheritdoc cref="ReleaseHandle"/>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            // Release the FDB handle.
            fdb_future_destroy(handle);

            return true;
        }

        [DllImport(FdbConstants.FdbDll)]
        [SuppressUnmanagedCodeSecurity]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static extern void fdb_future_destroy(IntPtr future);
    }
}
