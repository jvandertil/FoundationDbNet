namespace FoundationDbNet.Layers.Tuple.Tests.Framework
{
    using System;
    using FoundationDbNet.Layers.Tuple;
    using NFluent;

    public abstract class FdbTupleTestBase
    {
        protected void EncodeAndVerify(Func<FdbTuple, FdbTuple> adder, ByteString expected)
            => EncodeAndVerify(adder, expected.ToArray());

        protected void EncodeAndVerify(Func<FdbTuple, FdbTuple> adder, byte[] expected)
        {
            var result = adder(new FdbTuple()).Pack().ToArray();

            Check.That(result).IsEqualTo(expected);
        }
    }
}
