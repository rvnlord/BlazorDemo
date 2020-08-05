using System;

namespace BlazorDemo.Common.Converters
{
    public static class TimeConverter
    {
        public static DateTime UnixTimeStampToDateTime(this long unix) => new DateTime(1970, 1, 1).AddSeconds(unix);
    }
}
