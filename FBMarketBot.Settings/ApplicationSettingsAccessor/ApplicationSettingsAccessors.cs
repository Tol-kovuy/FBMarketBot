using FBMarketBot.Common;
using FBMarketBot.Settings.AppSettings;
using FBMarketBot.Shared;

namespace FBMarketBot.Settings.ApplicationSettingsAccessor
{
    /// <summary>
    /// Provides access to application settings by extending base accessor functionality.
    /// </summary>
    public class ApplicationSettingsAccessors
        : BaseAccessors<ApplicationSettings, IApplicationSettings>,
        IApplicationSettingsAccessor
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationSettingsAccessors class with the settings file path.
        /// </summary>
        public ApplicationSettingsAccessors()
            : base(FilePaths.RelativeSettingsFilePath)
        {
        }
    }
}