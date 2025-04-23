using FBMarketBot.Settings.Selectors.ListingFormSlctrs;
using FBMarketBot.Settings.Selectors.LoginSlctrs;

namespace FBMarketBot.Settings.Selectors
{
    /// <summary>
    /// Represents the interface for accessing all selector configurations within the application.
    /// This interface provides access to both login and listing form selectors.
    /// </summary>
    public interface ISelectors
    {
        /// <summary>
        /// Gets the login selectors configuration.
        /// </summary>
        ILoginSelectors LoginSelectors { get; }

        /// <summary>
        /// Gets the listing form selectors configuration.
        /// </summary>
        IListingFormSelectors ListingFormSelectors { get; }
    }
}