namespace FoundationDbNet.Native.Futures
{
    using System;

    internal delegate void FdbCallback(IntPtr futurePtr, IntPtr callbackParameterPtr);
}
