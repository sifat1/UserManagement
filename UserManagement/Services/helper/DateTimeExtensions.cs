using System;

namespace UserManagement.Helpers
{
    public static class DateTimeExtensions
    {
        public static string ToRelativeTime(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return "Never";

            var ts = DateTime.UtcNow - dateTime.Value.ToUniversalTime();

            if (ts.TotalDays >= 60)
                return $"{(int)(ts.TotalDays / 30)} months ago";
            if (ts.TotalDays >= 14)
                return $"{(int)(ts.TotalDays / 7)} weeks ago";
            if (ts.TotalDays >= 1)
                return $"{(int)ts.TotalDays} days ago";
            if (ts.TotalHours >= 1)
                return $"{(int)ts.TotalHours} hours ago";
            if (ts.TotalMinutes >= 1)
                return $"{(int)ts.TotalMinutes} minutes ago";

            return "just now";
        }
    }
}
