namespace FoundationDbNet.Tests.Features
{
    using System;
    using System.Threading.Tasks;
    using FoundationDbNet.Tests.Framework;
    using NFluent;
    using Xunit;

    [Collection(Traits.FoundationDB)]
    public class ReadSingleKeyTests
    {
        private readonly IFdbDatabase _db;

        public ReadSingleKeyTests(FdbFixture fixture)
        {
            _db = fixture.Database;
        }

        [Fact]
        public async Task CanReadKeyThatIsPresentInDatabase()
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

        [Fact]
        public async Task CanReadEmptyKeyThatIsPresentInDatabase()
        {
            var key = KeyHelper.GetRandomKey();

            using (var tx = _db.BeginTransaction())
            {
                tx.Set(key.Span, Array.Empty<byte>());

                await tx.CommitAsync();
            }

            FdbValue fromDb;
            using (var tx = _db.BeginSnapshotTransaction())
            {
                fromDb = await tx.GetAsync(key.Span);
            }

            Check.That(fromDb.IsPresent).IsTrue();
            Check.That(fromDb.Value).IsEqualTo(ReadOnlyMemory<byte>.Empty);
        }

        [Fact]
        public async Task CanReadKeyThatIsNotPresentInDatabase()
        {
            var key = KeyHelper.GetRandomKey();

            FdbValue fromDb;
            using (var tx = _db.BeginSnapshotTransaction())
            {
                fromDb = await tx.GetAsync(key.Span);
            }

            Check.That(fromDb.IsPresent).IsFalse();
            Check.That(fromDb.Value).IsEqualTo(ReadOnlyMemory<byte>.Empty);
        }
    }
}
