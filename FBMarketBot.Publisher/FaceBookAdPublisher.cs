using FBMarketBot.BrowserAutomation;
using FBMarketBot.CreatingListing;
using FBMarketBot.GoLogin;
using FBMarketBot.GoLogin.ProfileDTOs;
using FBMarketBot.Login;
using FBMarketBot.Settings.ApplicationSettingsAccessor;
using FBMarketBot.Settings.Schedule;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBMarketBot.Publisher
{
    /// <summary>
    /// Implements functionality to publish ads on Facebook by coordinating login and listing creation.
    /// </summary>
    public class FaceBookAdPublisher : IFaceBookAdPublisher
    {
        private readonly IFaceBookLogin _fbLogin;
        private readonly IListingService _createNewListingService;
        private readonly IWeeklyPostSchedule _weeklyPostSchedule;
        private readonly IScheduleHelper _scheduleHelper;
        private readonly IBrowserAutomation _browserAutomation;
        private readonly IGoLoginApiService _goLoginApiService;
        private readonly ILogger<FaceBookAdPublisher> _logger;

        /// <summary>
        /// Initializes a new instance of the FaceBookAdPublisher class with required dependencies.
        /// </summary>
        public FaceBookAdPublisher(
            IFaceBookLogin fbLogin,
            IListingService createNewListingService,
            IGoLoginApiService goLoginApiService,
            IBrowserAutomation browserAutomation,
            ILogger<FaceBookAdPublisher> logger,
            IScheduleHelper scheduleHelper)
        {
            _browserAutomation = browserAutomation;
            _goLoginApiService = goLoginApiService;
            _fbLogin = fbLogin;
            _createNewListingService = createNewListingService;
            _logger = logger;
            _scheduleHelper = scheduleHelper;
        }

        /// <summary>
        /// Asynchronously starts the process of logging in to Facebook and publishing ads.
        /// </summary>
        /// <returns>A task representing the asynchronous publishing operation.</returns>
        public async Task StartByScheduleAsync()
        {
            try
            {
                while (true)
                {
                    var now = DateTime.Now;
                    var todaySchedule = _scheduleHelper.GetTodaySchedule(now.DayOfWeek);

                    if (todaySchedule == null || !todaySchedule.IsActive)
                    {
                        await _scheduleHelper.WaitUntilNextActiveDayAsync();
                        continue;
                    }

                    var nowAfter = DateTime.Now;
                    var todayPostTime = nowAfter.Date + todaySchedule.PostingTime;

                    if (now < todayPostTime)
                    {
                        var delay = todayPostTime - now;
                        _logger.LogInformation($"Waiting for posting time: {delay.TotalHours:F1} hours...");
                        await Task.Delay(delay);
                    }

                    var profileIds = _scheduleHelper.GetTodayProfileIds();

                    foreach (var profileId in profileIds)
                    {
                        var profile = await _goLoginApiService.GetProfileByIdAsync(profileId);

                        await _browserAutomation.InitializeAsync(profile);

                        for (int i = 0; i < todaySchedule.PostsCount; i++)
                        {
                            await _fbLogin.LoginAsync();
                            await _createNewListingService.CreateAsync(profile);
                        }

                        await _goLoginApiService.StopProfileAsync(profile);
                    }

                    
                    _logger.LogInformation("Publications are complete. Waiting for the next active day...");

                    await _scheduleHelper.WaitUntilNextActiveDayAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error in the process of publishing announcements.");
                throw;
            }
        }
    }
}