using FBMarketBot.Settings.Categories;
using FBMarketBot.Settings.Login;
using FBMarketBot.Settings.Proxy;
using FBMarketBot.Settings.Schedule;
using System;

namespace FBMarketBot.Settings.AppSettings
{
    /// <summary>
    /// Represents the application-wide settings, implementing the IApplicationSettings interface.
    /// </summary>
    public class ApplicationSettings : IApplicationSettings
    {
        /// <summary>
        /// Gets or sets the login credentials for Facebook authentication.
        /// </summary>
        public LoginCredentials LoginCredentials { get; set; }

        public string GoLoginApiToken { get; set; }

        /// <summary>
        /// Gets the login credentials as an ILoginCredentials interface for interface implementation.
        /// </summary>
        ILoginCredentials IApplicationSettings.LoginCredentials { get => LoginCredentials; }

        /// <summary>
        /// Gets or sets the settings for configuring listing forms.
        /// </summary>
        public ListingFormSettings ListingFormSettings { get; set; }

        /// <summary>
        /// Gets the listing form settings as an IListingFormSettings interface for interface implementation.
        /// </summary>
        IListingFormSettings IApplicationSettings.ListingFormSettings { get => ListingFormSettings; }

        /// <summary>
        /// Gets or sets the weekly posting schedule for automated listing creation.
        /// </summary>
        public WeeklyPostSchedule WeeklyPostSchedule { get; set; }

        /// <summary>
        /// Gets the weekly posting schedule as an IWeeklyPostSchedule interface for interface implementation.
        /// </summary>
        IWeeklyPostSchedule IApplicationSettings.WeeklyPostSchedule { get => WeeklyPostSchedule; }

        /// <summary>
        /// Gets or sets the minimum delay between actions for simulating human-like behavior.
        /// </summary>
        public TimeSpan DelayBetweenActionsFrom { get; set; }

        /// <summary>
        /// Gets or sets the maximum delay between actions for simulating human-like behavior.
        /// </summary>
        public TimeSpan DelayBetweenActionsTo { get; set; }

        /// <summary>
        /// Gets or sets the proxy settings for network requests.
        /// </summary>
        public ProxySettings ProxySettings { get; set; }

        /// <summary>
        /// Gets the proxy settings as an IProxySettings interface for interface implementation.
        /// </summary>
        IProxySettings IApplicationSettings.ProxySettings { get => ProxySettings; }
    }
}