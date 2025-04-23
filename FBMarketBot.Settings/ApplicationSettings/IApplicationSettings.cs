using FBMarketBot.Settings.Categories;
using FBMarketBot.Settings.Login;
using FBMarketBot.Settings.Proxy;
using FBMarketBot.Settings.Schedule;
using System;

namespace FBMarketBot.Settings.AppSettings
{
    /// <summary>
    /// Defines a contract for accessing application-wide settings.
    /// </summary>
    public interface IApplicationSettings
    {
        /// <summary>
        /// Gets the login credentials for Facebook authentication.
        /// </summary>
        ILoginCredentials LoginCredentials { get; }

        string GoLoginApiToken { get; }

        /// <summary>
        /// Gets the settings for configuring listing forms.
        /// </summary>
        IListingFormSettings ListingFormSettings { get; }

        /// <summary>
        /// Gets the weekly posting schedule for automated listing creation.
        /// </summary>
        IWeeklyPostSchedule WeeklyPostSchedule { get; }

        /// <summary>
        /// Gets the minimum delay between actions for simulating human-like behavior.
        /// </summary>
        TimeSpan DelayBetweenActionsFrom { get; }

        /// <summary>
        /// Gets the maximum delay between actions for simulating human-like behavior.
        /// </summary>
        TimeSpan DelayBetweenActionsTo { get; }

        /// <summary>
        /// Gets the proxy settings for network requests.
        /// </summary>
        IProxySettings ProxySettings { get; }
    }
}