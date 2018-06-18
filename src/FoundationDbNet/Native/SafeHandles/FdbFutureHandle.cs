namespace FoundationDbNet.Native.SafeHandles
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a wrapper class for a FoundationDB future handle.
    /// </summary>
    /// <remarks>
    /// A FoundationDB future represents an asynchronous operation in unmanaged code.
    /// </remarks>
    public sealed class FdbFutureHandle : SafeHandle
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
            NativeMethods.fdb_future_destroy(handle);

            return true;
        }
    }
}
