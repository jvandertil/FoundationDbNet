namespace FoundationDbNet.Layers.Tuple.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FoundationDbNet.Layers.Tuple.Tests.Framework;
    using Xunit;

    public class FdbTupleEncodingTests : FdbTupleTestBase
    {
        [Theory]
        [InlineData(true, "0x27")]
        [InlineData(false, "0x26")]
        public void CanEncodeBoolean(bool value, string expected)
            => EncodeAndVerify(t => t.Add(value), expected);

        [Theory]
        [InlineData(0, "0x14")] // Technically not positive, but whatever ;).
        [InlineData(128, "0x1580")]
        [InlineData(0xFF, "0x15FF")]
        [InlineData(16801, "0x1641A1")]
        [InlineData(0xFFFF, "0x16FFFF")]
        [InlineData(14727200, "0x17E0B820")]
        [InlineData(0xFFFFFF, "0x17FFFFFF")]
        [InlineData(1455720000, "0x1856C48640")]
        [InlineData(0xFFFFFFFF, "0x18FFFFFFFF")]
        [InlineData(0x7FFFFFFFFFFFFFFF, "0x1C7FFFFFFFFFFFFFFF")]

        public void CanEncodePositiveLongValues(long value, string expected)
            => EncodeAndVerify(t => t.Add(value), expected);

        [Theory]
        [InlineData(-1, "0x13FE")]
        [InlineData(-128, "0x137F")]
        [InlineData(-0xFF, "0x1300")]
        [InlineData(-16801, "0x12BE5E")]
        [InlineData(-0xFFFF, "0x120000")]
        [InlineData(-14727200, "0x111F47DF")]
        [InlineData(-0xFFFFFF, "0x11000000")]
        [InlineData(-1455720000, "0X10A93B79BF")]
        [InlineData(-0xFFFFFFFF, "0x1000000000")]
        [InlineData(-6273172034854775807, "0X0CA8F137455F911400")]
        [InlineData(-9223372036854775808, "0X0C7FFFFFFFFFFFFFFF")]
        public void CanEncodeNegativeLongValues(long value, string expected)
            => EncodeAndVerify(t => t.Add(value), expected);

        [Theory]
        [InlineData(double.NaN, "0X210007FFFFFFFFFFFF")]
        [InlineData(10.101, "0X21C02433B645A1CAC1")]
        [InlineData(10, "0x21c024000000000000")]
        [InlineData(1.12333121337e+07, "0x21c1656d0404474539")]
        public void CanEncodeDouble(double value, string expected)
            => EncodeAndVerify(t => t.Add(value), expected);

        [Theory]
        [InlineData(new byte[0], "0x0100")]
        [InlineData(new byte[] { 0x02, 0x03, 0x41, 0x11 }, "0x010203411100")]
        [InlineData(new byte[] { 0x02, 0x03, 0x00, 0x11 }, "0x01020300FF1100")]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, "0x0100FF00FF00FF00FF00")]
        public void CanEncodeByteArray(byte[] data, string expected)
            => EncodeAndVerify(t => t.Add(data), expected);

        [Theory]
        [InlineData(0, "0X2080000000")]
        [InlineData(1, "0X20BF800000")]
        [InlineData(-1, "0X20407FFFFF")]
        [InlineData(float.NaN, "0X20003FFFFF")]
        [InlineData(12.11F, "0X20C141C28F")]
        [InlineData(float.PositiveInfinity, "0X20FF800000")]
        [InlineData(float.NegativeInfinity, "0X20007FFFFF")]
        public void CanEncodeFloat(float value, string expected)
            => EncodeAndVerify(t => t.Add(value), expected);

        [Theory]
        [InlineData(null, "0x00")]
        [InlineData("", "0x0200")]
        [InlineData("FOObar", "0x02464f4f62617200")]
        [InlineData("F\u00d4O\u0000bar", "0x0246c3944f00ff62617200")]
        public void CanEncodeString(string value, string expected)
            => EncodeAndVerify(t => t.Add(value), expected);

        [Theory]
        [MemberData(nameof(TestData))]
        public void CanEncodeGuid(Guid value, string expected)
            => EncodeAndVerify(t => t.Add(value), expected);

        [Theory]
        [InlineData(new object[] { null }, "0x0500ff00")]
        [InlineData(new object[] { "test1234567890" }, "0x050274657374313233343536373839300000")]
        [InlineData(new object[] { "test", 12345L, null, 10.1234D }, "0x0502746573740016303900ff21c0243f2e48e8a71e00")]
        public void CanEncodeFdbTuple(object[] values, string expected)
        {
            var tuple = new FdbTuple();

            foreach (var value in values)
            {
                AddWithReflection(value);
            }

            EncodeAndVerify(t => t.Add(tuple), expected);

            void AddWithReflection(object value)
            {
                var methods = typeof(FdbTuple)
                    .GetMethods()
                    .Where(x => x.Name == "Add");

                var typeOfValue = value == null ? typeof(string) : value.GetType();

                var method = methods
                    .Where(x => x.GetParameters()[0].ParameterType.IsAssignableFrom(typeOfValue))
                    .Single();

                method.Invoke(tuple, new object[] { value });
            }
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { Guid.Parse("00010203-0405-0607-0809-0a0b0c0d0e0f"), "0x30000102030405060708090a0b0c0d0e0f" };

            var value = Guid.NewGuid();

            yield return new object[] { value, "0x30" + value.ToString("N") };
        }
    }
}
