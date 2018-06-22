namespace FoundationDbNet.Tests
{
    using System;
    using Xunit;

    public sealed class FdbFixture : IDisposable
    {
        public Fdb Fdb { get; }

        private bool _disposed;

        public FdbFixture()
        {
            Fdb = Fdb.Instance;
            Fdb.Initialize(FdbTestConstants.ApiVersion);

            _disposed = false;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            Fdb.Terminate();

            _disposed = true;
        }
    }

    [CollectionDefinition("Fdb")]
    public class FdbCollection : ICollectionFixture<FdbFixture> { }
}
