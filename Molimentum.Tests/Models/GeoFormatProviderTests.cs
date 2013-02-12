using System;
using System.Globalization;
using FluentAssertions;
using Molimentum.Models;
using Xunit;

namespace Molimentum.Tests.Models
{
    public class GeoFormatProvideTests
    {
        [Fact]
        public void Can_Format_Latitude()
        {
            var latitude = 3.6;
            var expectedString = "03°36.0'N".Replace(".", NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);

            var result = String.Format(GeoFormatProvider.Latitude, "{0}", latitude);

            result.Should().Be(expectedString);
        }

        [Fact]
        public void Can_Format_Longitude()
        {
            var longitude = 10.6;
            var expectedString = "010°36.0'E".Replace(".", NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);

            var result = String.Format(GeoFormatProvider.Longitude, "{0}", longitude);

            result.Should().Be(expectedString);
        }
    }
}
