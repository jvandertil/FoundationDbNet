namespace FoundationDbNet.Tests
{
    using Xunit;

    [CollectionDefinition(Traits.FoundationDB)]
    public class FoundationDbCollection : ICollectionFixture<FdbFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
