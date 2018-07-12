namespace FoundationDbNet.Tests.Scenarios
{
    using System.Threading.Tasks;
    using Xunit;

    public class CommitEmptyTransaction : FdbDatabaseTestBase
    {
        public CommitEmptyTransaction(FdbFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task DoesNotThrowException()
        {
            using (var tx = Db.BeginTransaction())
            {
                await tx.CommitAsync();
            }
        }
    }
}
