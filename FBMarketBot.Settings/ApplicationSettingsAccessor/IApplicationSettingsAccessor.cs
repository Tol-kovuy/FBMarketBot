using FBMarketBot.IoC;
using FBMarketBot.Settings.AppSettings;

namespace FBMarketBot.Settings.ApplicationSettingsAccessor
{
    /// <summary>
    /// Defines a contract for accessing application settings with transient lifetime dependency registration.
    /// </summary>
    public interface IApplicationSettingsAccessor : ITransientDependency
    {
        /// <summary>
        /// Retrieves the current application settings.
        /// </summary>
        /// <returns>The application settings as an IApplicationSettings instance.</returns>
        IApplicationSettings Get();
    }
}