namespace FoundationDbNet.Tests
{
    using System;
    using System.Threading.Tasks;
    using FdbServer;
    using FoundationDbNet.Extensions;
    using Xunit;

    public sealed class FdbFixture : IAsyncLifetime
    {
        private IFdbServer _server;

        public string ClusterFile => _server.ClusterFile;

        public Fdb Fdb { get; }

        public IFdbDatabase Database { get; private set; }

        public FdbFixture()
        {
            Fdb = Fdb.Instance;
            Fdb.SelectApiVersion(520);
        }

        public async Task InitializeAsync()
        {
            var builder = new FdbServerBuilder()
                .WithVersion(FdbServerVersion.v5_2_5);

            _server = await builder.BuildAsync();

            _server.Start();
            _server.Initialize();

            var db = await Fdb.OpenAsync(ClusterFile);
            Database = db;

            using (var tx = Database.BeginSnapshotTransaction())
            {
                await tx.GetAsync(new byte[] { 0x00 });
            }
        }

        public Task DisposeAsync()
        {
            try { Database.Dispose(); } catch { }
            try { Fdb.Terminate(); } catch { }

            _server.Stop();

            Exception caughtException = null;
            do
            {
                try
                {
                    _server.Destroy();
                }
                catch (Exception e)
                {
                    caughtException = e;
                }
            }
            while (caughtException != null);

            return Task.CompletedTask;
        }
    }
}
