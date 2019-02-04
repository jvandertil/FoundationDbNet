using System;
using System.Linq;
using System.Runtime.InteropServices;
using FoundationDbNet.Layers.Tuple.Tests.Framework;
using Xunit;

namespace FoundationDbNet.Layers.Tuple.Tests
{
    public class TupleParserTests
    {
        [Fact]
        public void Test()
        {
            var tuple = new ByteString("0x30000102030405060708090A0B0C0D0E0F1C7FFFFFFFFFFFFFFF");
            var encodedTuple = new EncodedFdbTuple(tuple.ToArray());

            var decodedGuid = encodedTuple.Get<Guid>(0);
            var decodedLong = encodedTuple.Get<long>(1);

            Assert.Equal("00010203-0405-0607-0809-0a0b0c0d0e0f", decodedGuid.ToString());
            Assert.Equal(long.MaxValue, decodedLong);
        }
    }
}
