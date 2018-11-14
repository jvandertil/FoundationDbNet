namespace FoundationDbNet.Native.Futures
{
    using FoundationDbNet.Native.SafeHandles;

    internal sealed class FdbVoidFuture : FdbFuture<Unit>
    {
        public FdbVoidFuture(FdbFutureHandle futureHandle)
            : base(futureHandle)
        {
        }

        protected override Unit GetResult()
        {
            // No op
            return Unit.Value;
        }
    }
}
