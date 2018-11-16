namespace FoundationDbNet.Tests.Framework.Async
{
    using System;
    using System.Threading.Tasks;

    public class AsyncTester
    {
        public static void CheckForDeadlockOnSingleThread(Func<Task> test)
        {
            new DedicatedThreadSynchronisationContext().Send(state =>
            {
                test().Wait();
            }, null);
        }
    }
}
