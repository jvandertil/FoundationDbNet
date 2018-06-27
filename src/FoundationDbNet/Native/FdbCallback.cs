namespace FoundationDbNet.Native
{
    using System;

    internal delegate void FdbCallback(IntPtr futurePtr, IntPtr callbackParameterPtr);
}
