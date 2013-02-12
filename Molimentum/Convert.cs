using System;

namespace Molimentum
{
    public static class Convert
    {
        public static readonly DateTimeOffset c_baseDateTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, new TimeSpan(0));

        public static DateTimeOffset? FromTimestamp(string s)
        {
            Int64 result;

            if (Int64.TryParse(s, out result)) return c_baseDateTime.AddMilliseconds(result);

            return null;
        }

        public static DateTimeOffset? FromTimestamp(ulong l)
        {
            return c_baseDateTime.AddMilliseconds(l);
        }

        public static DateTimeOffset? FromDateTime(DateTime? d)
        {
            if (d == null) return null;

            var dateTime = d.Value;

            return new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, new TimeSpan(0));
        }
    }
}