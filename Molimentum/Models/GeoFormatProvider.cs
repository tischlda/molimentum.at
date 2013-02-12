using System;

namespace Molimentum.Models
{
    public class GeoFormatProvider : IFormatProvider, ICustomFormatter
    {
        private readonly string _positiveSign;
        private readonly string _negativeSign;
        private readonly string _format;

        public static GeoFormatProvider Latitude { get; private set; }
        public static GeoFormatProvider Longitude { get; private set; }

        static GeoFormatProvider()
        {
            Latitude = new GeoFormatProvider(2, "N", "S");
            Longitude = new GeoFormatProvider(3, "E", "W");
        }

        public GeoFormatProvider(int degreeDigits, string positiveSign, string negativeSign)
        {
            _format = "{0:" + new String('0', degreeDigits) + "}°{1:00.0}'{2}";
            _positiveSign = positiveSign;
            _negativeSign = negativeSign;
        }

        public object GetFormat(Type formatType)
        {
            return (formatType == typeof(ICustomFormatter)) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (!(arg is Double)) throw new ArgumentException(String.Format("Unsupported type '{0}'.", arg.GetType()), "arg");

            var angle = (double)arg;

            var absoluteAngle = Math.Abs(angle);

            var degrees = Math.Floor(absoluteAngle);

            var minutes = (absoluteAngle - degrees) * 60;

            var sign = angle > 0 ? _positiveSign : _negativeSign;

            return String.Format(_format, degrees, minutes, sign);
        }
    }
}