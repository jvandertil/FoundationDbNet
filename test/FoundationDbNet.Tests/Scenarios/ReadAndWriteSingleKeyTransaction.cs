namespace FoundationDbNet.Tests.Scenarios
{
    using System.Text;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;

    public class ReadAndWriteSingleKeyTransaction : FdbDatabaseTestBase
    {
        public ReadAndWriteSingleKeyTransaction(FdbFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task DoesNotThrowException()
        {
            byte[] testKey = Encoding.UTF8.GetBytes("ab");
            byte[] testValue = { 0x13, 0x37, 0x12, 0x34 };

            using (var tx = Db.BeginTransaction())
            {
                tx.Set(testKey, testValue);

                await tx.CommitAsync();
            }

            using (var tx = Db.BeginTransaction())
            {
                var value = await tx.Get(testKey);

                value.ShouldBe(testValue);

                await tx.CommitAsync();
            }
        }
    }
}
