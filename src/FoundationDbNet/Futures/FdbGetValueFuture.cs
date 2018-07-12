namespace FoundationDbNet.Futures
{
    using System;
    using System.Buffers;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using FoundationDbNet.Native;
    using FoundationDbNet.Native.SafeHandles;

    internal sealed class FdbGetValueFuture : FdbFuture<byte[]>
    {
        public FdbGetValueFuture(FdbFutureHandle futureHandle) 
            : base(futureHandle)
        {
        }

        protected override byte[] GetResult()
        {
            NativeMethods.fdb_future_get_value(Handle, out bool present, out IntPtr valueBuffer, out int len);

            if (present)
            {
                if (len == 0)
                {
                    return Array.Empty<byte>();
                }

                byte[] result = new byte[len];

                Marshal.Copy(valueBuffer, result, 0, len);

                return result;
            }

            return null;
        }
    }
}
