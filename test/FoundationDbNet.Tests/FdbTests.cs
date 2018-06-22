namespace FoundationDbNet.Tests
{
    using System;
    using Shouldly;
    using Xunit;

    public class FdbTests
    {
        // Given
        private const int ApiVersion = FdbTestConstants.ApiVersion;

        private readonly Fdb _fdb = Fdb.Instance;

        public class TheInitializeMethod : FdbTests
        {
            public TheInitializeMethod()
            {
                // When
                _fdb.Initialize(ApiVersion);
            }

            [Fact]
            public void WhenCalledTwiceWithSameAPIVersion_DoesNotThrow()
            {
                // Then
                Should.NotThrow(() => _fdb.Initialize(ApiVersion));
            }

            [Fact]
            public void WhenCalledTwiceWithDifferentAPIVersions_ThrowsInvalidOperationException()
            {
                // Then
                var exception = Should.Throw<InvalidOperationException>(() => _fdb.Initialize(ApiVersion + 1));

                exception.Message.ShouldContain("Initialize was called multiple times. This is not supported");
            }

            [Fact]
            public void WhenCalled_SetsInitializedToTrue()
            {
                // Then
                _fdb.Initialized.ShouldBeTrue();
            }

            [Fact]
            public void WhenCalled_SetsIsAliveToTrue()
            {
                // Then
                _fdb.IsAlive.ShouldBeTrue();
            }
        }
    }
}
