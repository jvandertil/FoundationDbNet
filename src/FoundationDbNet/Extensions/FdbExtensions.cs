namespace FoundationDbNet.Extensions
{
    using System.Threading.Tasks;

    public static class FdbExtensions
    {
        public static async Task<IFdbDatabase> OpenDefaultAsync(this Fdb fdb)
        {
            using (var cluster = await fdb.OpenDefaultClusterAsync().ConfigureAwait(false))
            {
                var database = await cluster.OpenDefaultDatabaseAsync().ConfigureAwait(false);

                return database;
            }
        }

        public static async Task<IFdbDatabase> OpenAsync(this Fdb fdb, string clusterFile)
        {
            using (var cluster = await fdb.OpenClusterAsync(clusterFile).ConfigureAwait(false))
            {
                var database = await cluster.OpenDefaultDatabaseAsync().ConfigureAwait(false);

                return database;
            }
        }
    }
}
