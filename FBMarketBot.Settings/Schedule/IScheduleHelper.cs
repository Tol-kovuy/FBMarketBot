using FBMarketBot.IoC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FBMarketBot.Settings.Schedule
{
    public interface IScheduleHelper : ITransientDependency
    {
        IDaySchedule GetTodaySchedule(DayOfWeek day);
        IList<IDaySchedule> GetWeekSchedule();
        List<string> GetTodayProfileIds();
        Task WaitUntilNextActiveDayAsync();
    }
}
