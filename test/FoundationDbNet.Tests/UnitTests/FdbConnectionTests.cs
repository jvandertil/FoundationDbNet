using System.Threading.Tasks;
using Shouldly;

namespace FoundationDbNet.Tests.UnitTests
{
    using Xunit;

    [Collection("Fdb")]
    [Trait("RequiresFdbInstall", "true")]
    public class FdbConnectionTests
    {
        private readonly Task<FdbConnection> _connection;

        public FdbConnectionTests(FdbFixture fixture)
        {
            _connection = fixture.Fdb.OpenClusterAsync(FdbTestConstants.TestClusterFile);
        }

        public class TheOpenDefaultDatabaseAsyncMethod : FdbConnectionTests
        {
            public TheOpenDefaultDatabaseAsyncMethod(FdbFixture fixture)
                : base(fixture)
            {
            }

            [Fact]
            public async Task ReturnsAnFdbDatabaseAsync()
            {
                var database = await _connection;

                database.ShouldNotBeNull();
            }
        }
    }
}
