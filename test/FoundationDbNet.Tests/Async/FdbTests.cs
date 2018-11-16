namespace FoundationDbNet.Tests.Async
{
    using FoundationDbNet.Extensions;
    using FoundationDbNet.Tests.Framework;
    using FoundationDbNet.Tests.Framework.Async;
    using Xunit;

    [Collection(Traits.FoundationDB)]
    public class FdbTests
    {
        private readonly FdbFixture _fixture;

        public FdbTests(FdbFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestOpenClusterAsync_DoesNotDeadlock()
        {
            AsyncTester.CheckForDeadlockOnSingleThread(() => Fdb.Instance.OpenClusterAsync(_fixture.ClusterFile));
        }
    }

    [Collection(Traits.FoundationDB)]
    public class FdbExtensionsTests
    {
        private readonly FdbFixture _fixture;

        public FdbExtensionsTests(FdbFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestOpenAsync_DoesNotDeadlock()
        {
            AsyncTester.CheckForDeadlockOnSingleThread(() => Fdb.Instance.OpenAsync(_fixture.ClusterFile));
        }
    }
}
