namespace FoundationDbNet.Tests.Scenarios
{
    using System;
    using System.Threading.Tasks;
    using Xunit;

    [Collection("Fdb")]
    [Trait("RequiresFdbInstall", "true")]
    public abstract class FdbDatabaseTestBase : IAsyncLifetime, IDisposable
    {
        private readonly Fdb _fdb;

        private FdbConnection _connection;

        protected FdbDatabase Db { get; private set; }

        protected FdbDatabaseTestBase(FdbFixture fixture)
        {
            _fdb = fixture.Fdb;
        }

        public async Task InitializeAsync()
        {
            _connection = await _fdb.OpenClusterAsync(FdbTestConstants.TestClusterFile);
            Db = await _connection.OpenDefaultDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        public void Dispose()
        {
            _connection?.Dispose();
            Db?.Dispose();

            _connection = null;
            Db = null;
        }
    }
}
