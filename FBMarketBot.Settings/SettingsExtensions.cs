using FBMarketBot.Settings.ApplicationSettingsAccessor;
using System;
using System.Threading.Tasks;

namespace FBMarketBot.Settings
{
    /// <summary>
    /// Provides extension methods for application settings-related functionality.
    /// </summary>
    public static class SettingsExtensions
    {
        /// <summary>
        /// Asynchronously introduces a random delay between actions based on configured time ranges.
        /// </summary>
        /// <param name="settings">The application settings accessor providing delay configuration.</param>
        /// <returns>A task representing the asynchronous delay operation.</returns>
        public static async Task DelayBetweenActionsAsync(this IApplicationSettingsAccessor settings)
        {
            var config = settings.Get();
            var random = new Random();

            int minMs = (int)config.DelayBetweenActionsFrom.TotalMilliseconds;
            int maxMs = (int)config.DelayBetweenActionsTo.TotalMilliseconds;

            int delayMs = random.Next(minMs, maxMs);

            await Task.Delay(delayMs);
        }
    }
}