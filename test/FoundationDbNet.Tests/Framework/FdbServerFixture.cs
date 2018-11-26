namespace FoundationDbNet.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FdbServer;
    using FoundationDbNet.Extensions;
    using Xunit;

    public sealed class FdbFixture : IAsyncLifetime
    {
        private readonly FdbServerVersion _serverVersion;

        private IFdbServer _server;

        public string ClusterFile => _server.ClusterFile;

        public Fdb Fdb { get; }

        public IFdbDatabase Database { get; private set; }

        public FdbFixture()
        {
            const int fallbackFdbApiVersion = 520;

            var apiVersion = Environment.GetEnvironmentVariable("FdbApiVersion");
            var serverVersion = Environment.GetEnvironmentVariable("FdbServerVersion");

            if (apiVersion == null
                || !int.TryParse(apiVersion, out int fdbApiVersion))
            {
                fdbApiVersion = fallbackFdbApiVersion;
            }

            var availableVersions = Enum.GetNames(typeof(FdbServerVersion));
            if (serverVersion != null
                && availableVersions.Contains(serverVersion))
            {
                _serverVersion = (FdbServerVersion)Enum.Parse(typeof(FdbServerVersion), serverVersion);
            }
            else
            {
                _serverVersion = FdbServerVersion.v6_0_15;
            }

            EnsureApiVersionAndServerVersionCompatible(fdbApiVersion, _serverVersion);

            Fdb = Fdb.Instance;
            Fdb.SelectApiVersion(fdbApiVersion);
        }

        private void EnsureApiVersionAndServerVersionCompatible(int fdbApiVersion, FdbServerVersion serverVersion)
        {
            var normalizedServerVersion = serverVersion.ToString().Substring(1).Replace("_", string.Empty);

            if (fdbApiVersion.ToString().CompareTo(normalizedServerVersion) > 0)
            {
                throw new InvalidOperationException($"FdbApiVersion {fdbApiVersion} is not compatible with FdbServerVersion {serverVersion}.");
            }

            var maxApiVersion = Fdb.Instance.GetMaxApiVersion();

            if (maxApiVersion.ToString().CompareTo(normalizedServerVersion) > 0)
            {
                throw new InvalidOperationException($"Native library has max version {maxApiVersion} and is not compatible with FdbServerVersion {serverVersion}.");
            }
        }

        public async Task InitializeAsync()
        {
            var builder = new FdbServerBuilder()
                .WithVersion(_serverVersion);

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
