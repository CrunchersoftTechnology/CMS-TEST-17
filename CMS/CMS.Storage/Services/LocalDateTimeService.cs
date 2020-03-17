using System;

namespace CMS.Domain.Storage.Services
{
  public  class LocalDateTimeService :ILocalDateTimeService
    {
        public DateTime GetDateTime()
        {
            // DateTime serverDateTime = DateTime.UtcNow;
            DateTime dbDateTime = DateTime.UtcNow;

            //get date time offset for UTC date stored in the database
            DateTimeOffset dbDateTimeOffset = new
                            DateTimeOffset(dbDateTime, TimeSpan.Zero);

            //get user's time zone from profile stored in the database
            TimeZoneInfo userTimeZone =
                         TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            //convert  db offset to user offset
            DateTimeOffset userDateTimeOffset =
                       TimeZoneInfo.ConvertTime
                      (dbDateTimeOffset, userTimeZone);

            DateTime userDateTimeString = userDateTimeOffset.DateTime;

            return userDateTimeString;
        }
    }
}
