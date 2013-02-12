using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Molimentum.Models
{
    public class Position
    {
        private const string c_latitudePattern = @"(?<latitudeDegrees>\d+)°(?<latitudeMinutes>\d+\.\d+)'(?<latitudeSign>[NS])";
        private const string c_longitudePattern = @"(?<longitudeDegrees>\d+)°(?<longitudeMinutes>\d+\.\d+)'(?<longitudeSign>[EW])";

        private static readonly Regex s_positionPattern = new Regex(@"^" + c_latitudePattern + " " + c_longitudePattern + "$");
        private static readonly Regex s_latitudePattern = new Regex(@"^" + c_latitudePattern + "$");
        private static readonly Regex s_longitudePattern = new Regex(@"^" + c_longitudePattern + "$");

        public Position()
        {

        }

        public Position(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Position);
        }


        public bool Equals(Position other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(other, null)) return false;
            
            // about 1 cm
            const double epsilon = 0.0000001;

            return Math.Abs(Latitude - other.Latitude) < epsilon &&
                   Math.Abs(Longitude - other.Longitude) < epsilon;
        }


        public override int GetHashCode()
        {
            return Latitude.GetHashCode() ^ Longitude.GetHashCode();
        }


        public static bool TryParse(string s, out Position result)
        {
            result = null;

            if (s == null) return false;

            var positionMatch = s_positionPattern.Match(s);

            return TryParse(positionMatch, positionMatch, out result);
        }


        public static bool TryParse(string latitudeString, string longitudeString, out Position result)
        {
            result = null;

            if (latitudeString == null) return false;
            if (longitudeString == null) return false;

            var latitudeMatch = s_latitudePattern.Match(latitudeString);
            var longitudeMatch = s_longitudePattern.Match(longitudeString);

            return TryParse(latitudeMatch, longitudeMatch, out result);
        }


        private static bool TryParse(Match latitudeMatch, Match longitudeMatch, out Position result)
        {
            result = null;

            if (!latitudeMatch.Success) return false;
            if (!longitudeMatch.Success) return false;

            var latitudeDegrees = double.Parse(latitudeMatch.Groups["latitudeDegrees"].Value, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
            var latitudeMinutes = double.Parse(latitudeMatch.Groups["latitudeMinutes"].Value, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
            var latitude = latitudeDegrees + latitudeMinutes / 60;
            if (latitudeMatch.Groups["latitudeSign"].Value == "S") latitude *= -1;

            var longitudeDegrees = double.Parse(longitudeMatch.Groups["longitudeDegrees"].Value, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
            var longitudeMinutes = double.Parse(longitudeMatch.Groups["longitudeMinutes"].Value, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
            var longitude = longitudeDegrees + longitudeMinutes / 60;
            if (longitudeMatch.Groups["longitudeSign"].Value == "W") longitude *= -1;

            result = new Position(latitude, longitude);

            return true;
        }
    }
}