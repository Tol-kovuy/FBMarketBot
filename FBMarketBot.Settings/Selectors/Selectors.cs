using FBMarketBot.Settings.Selectors.ListingFormSlctrs;
using FBMarketBot.Settings.Selectors.LoginSlctrs;

namespace FBMarketBot.Settings.Selectors
{
    /// <summary>
    /// Represents the implementation of the ISelectors interface.
    /// This class provides access to both login and listing form selectors.
    /// </summary>
    public class Selectors : ISelectors
    {
        /// <summary>
        /// Gets or sets the login selectors configuration.
        /// </summary>
        public LoginSelectors LoginSelectors { get; set; }

        /// <inheritdoc/>
        ILoginSelectors ISelectors.LoginSelectors { get => LoginSelectors; }

        /// <summary>
        /// Gets or sets the listing form selectors configuration.
        /// </summary>
        public ListingFormSelectors ListingFormSelectors { get; set; }

        /// <inheritdoc/>
        IListingFormSelectors ISelectors.ListingFormSelectors { get => ListingFormSelectors; }
    }
}