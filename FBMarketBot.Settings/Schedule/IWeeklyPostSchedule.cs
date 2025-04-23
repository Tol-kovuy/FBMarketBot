using FBMarketBot.IoC;

namespace FBMarketBot.Settings.Schedule
{
    /// <summary>
    /// Represents the interface for defining a weekly posting schedule.
    /// This interface provides access to the daily schedule settings for each day of the week.
    /// </summary>
    public interface IWeeklyPostSchedule : ITransientDependency
    {
        IDaySchedule Monday { get; }
        IDaySchedule Tuesday { get; }
        IDaySchedule Wednesday { get; }
        IDaySchedule Thursday { get; }
        IDaySchedule Friday { get; }
        IDaySchedule Saturday { get; }
        IDaySchedule Sunday { get; }
    }
}
