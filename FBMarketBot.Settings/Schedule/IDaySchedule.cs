using FBMarketBot.IoC;
using System;
using System.Collections.Generic;

namespace FBMarketBot.Settings.Schedule
{
    /// <summary>
    /// Represents the interface for defining the schedule settings for a specific day.
    /// This interface provides the structure for managing daily posting activities.
    /// </summary>
    public interface IDaySchedule : ITransientDependency
    {
        DayOfWeek Day { get; set; }
        bool IsActive { get; set;  }
        TimeSpan PostingTime { get; set; }
        int PostsCount { get; set; }
        IList<string> ProfileIDs { get; set; }
    }
}