namespace FoundationDbNet.Tests
{
    using System.Threading.Tasks;
    using FoundationDbNet.Tests.Framework;
    using NFluent;
    using Xunit;

    [Collection(Traits.FoundationDB)]
    public class WriteSingleKeyTests
    {
        private readonly IFdbDatabase _db;

        public WriteSingleKeyTests(FdbFixture fixture)
        {
            _db = fixture.Database;
        }

        [Fact]
        public async Task CanWriteKeyToDatabase()
        {
            var key = KeyHelper.GetRandomKey();
            var value = new byte[] { 0x01, 0x02, 0x03 };

            using (var tx = _db.BeginTransaction())
            {
                tx.Set(key.Span, value);

                await tx.CommitAsync();
            }

            FdbValue fromDb;
            using (var tx = _db.BeginSnapshotTransaction())
            {
                fromDb = await tx.GetAsync(key.Span);
            }

            Check.That(fromDb.IsPresent).IsTrue();
            Check.That(fromDb.Value.ToArray()).IsEqualTo(value);
        }
    }
}
