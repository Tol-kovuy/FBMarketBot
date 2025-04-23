using System;
using System.Collections.Generic;

namespace FBMarketBot.Settings.Schedule
{
    /// <summary>
    /// Represents the schedule settings for a specific day, implementing the IDaySchedule interface.
    /// This class defines the configuration for posting activities on a particular day.
    /// </summary>
    public class DaySchedule : IDaySchedule
    {
        public DayOfWeek Day { get; set; }
        public bool IsActive { get; set; }
        public int PostsCount { get; set; }
        public TimeSpan PostingTime { get; set; }
        public IList<string> ProfileIDs { get; set; }
    }
}
