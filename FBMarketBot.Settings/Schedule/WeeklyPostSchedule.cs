namespace FBMarketBot.Settings.Schedule
{
    /// <summary>
    /// Represents the class for defining a weekly posting schedule.
    /// This interface provides access to the daily schedule settings for each day of the week.
    /// </summary>
    public class WeeklyPostSchedule : IWeeklyPostSchedule
    {
        public DaySchedule Monday { get; set; }
        IDaySchedule IWeeklyPostSchedule.Monday => Monday;

        public DaySchedule Tuesday { get; set; }
        IDaySchedule IWeeklyPostSchedule.Tuesday => Tuesday;

        public DaySchedule Wednesday { get; set; }
        IDaySchedule IWeeklyPostSchedule.Wednesday => Wednesday;

        public DaySchedule Thursday { get; set; }
        IDaySchedule IWeeklyPostSchedule.Thursday => Thursday;

        public DaySchedule Friday { get; set; }
        IDaySchedule IWeeklyPostSchedule.Friday => Friday;

        public DaySchedule Saturday { get; set; }
        IDaySchedule IWeeklyPostSchedule.Saturday => Saturday;

        public DaySchedule Sunday { get; set; }
        IDaySchedule IWeeklyPostSchedule.Sunday => Sunday;
    }
}
