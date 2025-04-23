using FBMarketBot.Settings.ApplicationSettingsAccessor;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBMarketBot.Settings.Schedule
{
    public class SchedulerHelper : IScheduleHelper
    {
        private readonly ILogger<SchedulerHelper> _logger;
        private readonly IWeeklyPostSchedule _weeklySchedule;

        public SchedulerHelper(
            IWeeklyPostSchedule weeklySchedule,
            IApplicationSettingsAccessor settings,
            ILogger<SchedulerHelper> logger
            )
        {
            _weeklySchedule = settings.Get().WeeklyPostSchedule;
            _logger = logger;
        }

        public IDaySchedule GetTodaySchedule(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday: return _weeklySchedule.Monday;
                case DayOfWeek.Tuesday: return _weeklySchedule.Tuesday;
                case DayOfWeek.Wednesday: return _weeklySchedule.Wednesday;
                case DayOfWeek.Thursday: return _weeklySchedule.Thursday;
                case DayOfWeek.Friday: return _weeklySchedule.Friday;
                case DayOfWeek.Saturday: return _weeklySchedule.Saturday;
                case DayOfWeek.Sunday: return _weeklySchedule.Sunday;
                default: return null;
            }
        }

        public IList<IDaySchedule> GetWeekSchedule()
        {
            return new List<IDaySchedule>
            {
                _weeklySchedule.Monday,
                _weeklySchedule.Tuesday,
                _weeklySchedule.Wednesday,
                _weeklySchedule.Thursday,
                _weeklySchedule.Friday,
                _weeklySchedule.Saturday,
                _weeklySchedule.Sunday
            }.Where(d => d.IsActive).ToList();
        }

        public List<string> GetTodayProfileIds()
        {
            var dayMap = new Dictionary<DayOfWeek, List<string>>
            {
                { DayOfWeek.Monday, _weeklySchedule.Monday.ProfileIDs.ToList() },
                { DayOfWeek.Tuesday, _weeklySchedule.Tuesday.ProfileIDs.ToList() },
                { DayOfWeek.Wednesday, _weeklySchedule.Wednesday.ProfileIDs.ToList() },
                { DayOfWeek.Thursday, _weeklySchedule.Thursday.ProfileIDs.ToList() },
                { DayOfWeek.Friday, _weeklySchedule.Friday.ProfileIDs.ToList() },
                { DayOfWeek.Saturday, _weeklySchedule.Saturday.ProfileIDs.ToList() },
                { DayOfWeek.Sunday, _weeklySchedule.Sunday.ProfileIDs.ToList() }
            };

            if (dayMap.TryGetValue(DateTime.Today.DayOfWeek, out var profiles))
            {
                _logger.LogInformation($"Today You have {profiles.Count} profiles to posting.");
                return profiles.ToList();
            }

            _logger.LogInformation("No GoLogin profiles today. Please set profiles in _appStaticSettings.json");
            return new List<string>();
        }

        public async Task WaitUntilNextActiveDayAsync()
        {
            const int DaysInWeek = 7;

            while (true)
            {
                var now = DateTime.Now;
                var today = now.DayOfWeek;
                var weekSchedule = GetWeekSchedule();

                foreach (var schedule in weekSchedule)
                {
                    var numberOfDaysUntilNextActiveDay = ((int)schedule.Day - (int)today + DaysInWeek) % DaysInWeek;
                    var targetDate = now.Date.AddDays(numberOfDaysUntilNextActiveDay);
                    var scheduledTime = targetDate + schedule.PostingTime;

                    if (scheduledTime > now)
                    {
                        var delay = scheduledTime - now;
                        _logger.LogInformation($"Next posting day: {schedule.Day} at {schedule.PostingTime}. Waiting time: {delay.TotalHours:F1} hours.");
                        await Task.Delay(delay);
                        return;
                    }
                }

                _logger.LogWarning("No active days found in the next 7 days.");
                Environment.Exit(0);
            }
        }
    }
}
