using System;
namespace Trades
{
    public class TimeFormat
    {
        	


        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        public string TimeRemaining(DateTime yourDate){
              var ts = new TimeSpan(yourDate.Ticks- DateTime.Now.Ticks);
              double delta = Math.Abs(ts.TotalSeconds);

                if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second left" : ts.Seconds + " seconds left";

                if (delta < 2 * MINUTE)
                return "a minute left";

                if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes left";

                if (delta < 90 * MINUTE)
                return "an hour left";

                if (delta < 24 * HOUR)
                return ts.Hours + " hours left";

                if (delta < 48 * HOUR)
                return "2 days left";

                if (delta < 30 * DAY)
                return ts.Days + " days left";

                if (delta < 12 * MONTH)
                {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month left" : months + " months left";
                }
                else
                {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year left" : years + " years left";
                }
        }
    }
}