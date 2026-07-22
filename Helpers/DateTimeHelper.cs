using System;

namespace BestPriceStore.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime GetYemeniTime()
        {
            try
            {
                var zone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Aden");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    var zone = TimeZoneInfo.FindSystemTimeZoneById("Arabic Standard Time");
                    return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
                }
                catch
                {
                    // Fallback to manual offset of UTC+3 (Yemen does not observe DST)
                    return DateTime.UtcNow.AddHours(3);
                }
            }
        }
    }
}
