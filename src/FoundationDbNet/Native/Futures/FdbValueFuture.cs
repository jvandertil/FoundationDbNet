namespace FoundationDbNet.Native.Futures
{
    using System;
    using System.Runtime.InteropServices;
    using FoundationDbNet.Native.SafeHandles;

    internal sealed class FdbValueFuture : FdbFuture<FdbValue>
    {
        public FdbValueFuture(FdbFutureHandle futureHandle)
            : base(futureHandle)
        {
        }

        protected override FdbValue GetResult()
        {
            fdb_future_get_value(Handle, out bool present, out IntPtr valueBuffer, out int len)
                .EnsureSuccess();

            if (!present)
            {
                return FdbValue.NonExistent;
            }

            if (len == 0)
            {
                return FdbValue.Empty;
            }

            byte[] result = new byte[len];
            Marshal.Copy(valueBuffer, result, 0, len);

            return new FdbValue(true, result);
        }

        [DllImport(FdbConstants.FdbDll)]
        private static extern FdbError fdb_future_get_value(
            FdbFutureHandle future,
            [MarshalAs(UnmanagedType.Bool)] out bool present,
            out IntPtr value,
            out int len);
    }
}
