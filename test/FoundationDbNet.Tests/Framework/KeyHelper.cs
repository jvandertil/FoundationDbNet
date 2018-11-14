namespace FoundationDbNet.Tests.Framework
{
    using System;

    public static class KeyHelper
    {
        private static readonly Random _rng = new Random();

        public static ReadOnlyMemory<byte> GetRandomKey()
        {
            var key = new byte[16];

            lock (_rng)
            {
                _rng.NextBytes(key);
            }

            return key;
        }
    }
}
