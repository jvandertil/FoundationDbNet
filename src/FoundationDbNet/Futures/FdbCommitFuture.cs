namespace FoundationDbNet.Futures
{
    using FoundationDbNet.Native;
    using FoundationDbNet.Native.SafeHandles;

    internal sealed class FdbCommitFuture : FdbFuture<Unit>
    {
        public FdbCommitFuture(FdbFutureHandle futureHandle) 
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
