namespace FoundationDbNet.Futures
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using FoundationDbNet.Native;
    using FoundationDbNet.Native.SafeHandles;

    internal abstract class FdbFuture<T> : IDisposable
    {
        private readonly TaskCompletionSource<T> _tcs;

        private GCHandle _gch;
        private bool _disposed;

        protected FdbFutureHandle Handle { get; }

        protected FdbFuture(FdbFutureHandle futureHandle)
        {
            Handle = futureHandle;

            _tcs = new TaskCompletionSource<T>();
            _disposed = false;

            _gch = GCHandle.Alloc(this);
            NativeMethods.fdb_future_set_callback(futureHandle, SetResult, GCHandle.ToIntPtr(_gch));
        }

        ~FdbFuture() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _gch.Free();
            _gch = default;

            if (disposing)
            {
                Handle.Dispose();
            }

            _disposed = true;
        }

        public Task<T> ToTask()
        {
            return _tcs.Task;
        }

        protected abstract T GetResult();

        public static implicit operator Task<T>(FdbFuture<T> ftr)
        {
            return ftr.ToTask();
        }

        private static void SetResult(IntPtr futurePtr, IntPtr gchPtr)
        {
            var fdbFutureGch = GCHandle.FromIntPtr(gchPtr);

            try
            {
                FdbFuture<T> fdbFuture = (FdbFuture<T>)fdbFutureGch.Target;

                Debug.Assert(fdbFuture != null);
                Debug.Assert(futurePtr == fdbFuture.Handle.DangerousGetHandle());

                using (fdbFuture)
                {
                    var task = fdbFuture._tcs;
                    var handle = fdbFuture.Handle;

                    FdbError error = NativeMethods.fdb_future_get_error(handle);

                    if (error == FdbError.Success)
                    {
                        var result = fdbFuture.GetResult();

                        task.SetResult(result);
                    }
                    else
                    {
                        task.SetException(error.ToException());
                    }
                }
            }
            finally
            {
                fdbFutureGch.Free();
            }
        }
    }
}
