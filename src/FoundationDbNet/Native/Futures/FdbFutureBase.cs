namespace FoundationDbNet.Native.Futures
{
    using System;
    using System.Runtime.InteropServices;
    using FoundationDbNet.Native.SafeHandles;

    internal abstract class FdbFutureBase : IDisposable
    {
        private GCHandle _gch;
        private bool _disposed;

        protected FdbFutureHandle Handle { get; }

        protected FdbFutureBase(FdbFutureHandle handle)
        {
            Handle = handle ?? throw new ArgumentNullException(nameof(handle));

            _disposed = false;
        }

        ~FdbFutureBase()
        {
            Dispose(false);
        }

        public void DangerousRunSynchronously()
        {
            fdb_future_block_until_ready(Handle)
                .EnsureSuccess();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (_gch.IsAllocated)
            {
                _gch.Free();
            }

            if (disposing)
            {
                Handle.Dispose();
            }

            _disposed = true;
        }

        protected void RegisterCallback(FdbCallback callback)
        {
            bool callbackRegistered = false;
            _gch = GCHandle.Alloc(this);
            try
            {
                fdb_future_set_callback(Handle, callback, GCHandle.ToIntPtr(_gch))
                    .EnsureSuccess();

                callbackRegistered = true;
            }
            finally
            {
                if (!callbackRegistered)
                {
                    _gch.Free();
                    _gch = default;
                }
            }
        }

        protected FdbError GetError()
        {
            return fdb_future_get_error(Handle);
        }

        [DllImport(FdbConstants.FdbDll)]
        private static extern FdbError fdb_future_set_callback(
            FdbFutureHandle future,
            [MarshalAs(UnmanagedType.FunctionPtr)] FdbCallback callbackMethod,
            IntPtr callbackParameter);

        [DllImport(FdbConstants.FdbDll)]
        private static extern FdbError fdb_future_get_error(FdbFutureHandle future);

        [DllImport(FdbConstants.FdbDll)]
        private static extern FdbError fdb_future_block_until_ready(FdbFutureHandle future);
    }
}
