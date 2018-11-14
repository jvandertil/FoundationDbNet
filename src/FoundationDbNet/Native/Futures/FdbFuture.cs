namespace FoundationDbNet.Native.Futures
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using FoundationDbNet.Native.SafeHandles;

    internal abstract class FdbFuture<T> : FdbFutureBase
    {
        private readonly TaskCompletionSource<T> _tcs;

        protected FdbFuture(FdbFutureHandle handle)
            : base(handle)
        {
            _tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

            RegisterCallback(SetResult);
        }

        public Task<T> ToTask()
        {
            return _tcs.Task;
        }

        protected abstract T GetResult();

        private static void SetResult(IntPtr handle, IntPtr futurePtr)
        {
            var fdbFutureGch = GCHandle.FromIntPtr(futurePtr);
            var fdbFuture = (FdbFuture<T>)fdbFutureGch.Target;

            Debug.Assert(fdbFuture != null);

            using (fdbFuture)
            {
                TaskCompletionSource<T> task = fdbFuture._tcs;
                FdbError error = fdbFuture.GetError();

                if (error == FdbError.Success)
                {
                    T result = fdbFuture.GetResult();

                    task.SetResult(result);
                }
                else
                {
                    task.SetException(error.ToException());
                }
            }
        }
    }
}
