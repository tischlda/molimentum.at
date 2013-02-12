using FluentAssertions;
using Molimentum.Models;
using Xunit;
using Xunit.Extensions;

namespace Molimentum.Tests.Models
{
    public class PositionFixture
    {
        [Fact]
        public void EqualsTest()
        {
            var position1 = new Position(42, 15);
            var position2 = new Position(42, 15);

            var result = position1.Equals(position2);

            result.Should().BeTrue();
        }


        [Fact]
        public void NotEqualsTest1()
        {
            var position1 = new Position(42, 15);
            var position2 = new Position(42, 14);

            var result = position1.Equals(position2);

            result.Should().BeFalse();
        }


        [Fact]
        public void NotEqualsTest2()
        {
            var position1 = new Position(42, 15);
            var position2 = new Position(41, 15);

            var result = position1.Equals(position2);

            result.Should().BeFalse();
        }


        [Fact]
        public void EqualHashCodeTest()
        {
            var hashCode1 = new Position(42, 15).GetHashCode();
            var hashCode2 = new Position(42, 15).GetHashCode();

            hashCode1.Should().Be(hashCode2);
        }


        [Fact]
        public void UnEqualHashCodeTest()
        {
            var hashCode1 = new Position(42, 15).GetHashCode();
            var hashCode2 = new Position(42, 14).GetHashCode();

            hashCode1.Should().NotBe(hashCode2);
        }


        [Theory]
        [InlineData("01°00.00'N 002°00.00'E", 1, 2)]
        [InlineData("03°00.00'S 004°00.00'W", -3, -4)]
        public void ParsePositionTest(string s, double expectedLatitude, double expectedLongitude)
        {
            Position result;

            var success = Position.TryParse(s, out result);

            success.Should().BeTrue();
            result.Latitude.Should().BeInRange(expectedLatitude - 0.000000001, expectedLatitude + 0.000000001);
            result.Longitude.Should().BeInRange(expectedLongitude - 0.000000001, expectedLongitude + 0.000000001);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalid")]
        [InlineData("xx°00.00'N 002°00.00'E")]
        [InlineData("01°xx.xx'N 002°00.00'E")]
        [InlineData("01°00.00'x 002°00.00'E")]
        [InlineData("01°00.00'N xxx°00.00'E")]
        [InlineData("01°00.00'N 002°xx.xx'E")]
        [InlineData("01°00.00'N 002°00.00'x")]
        public void ParseInvalidPositionTest(string s)
        {
            Position result;

            var success = Position.TryParse(s, out result);

            success.Should().BeFalse();
        }
    }
}
