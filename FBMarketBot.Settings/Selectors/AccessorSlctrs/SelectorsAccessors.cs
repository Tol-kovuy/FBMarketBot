using FBMarketBot.Common;
using FBMarketBot.Shared;

namespace FBMarketBot.Settings.Selectors.AccessorSlctrs
{
    /// <summary>
    /// Provides access to selectors by inheriting from BaseAccessors and implementing ISelectorsAccessors.
    /// This class is responsible for managing the retrieval of selector configurations.
    /// </summary>
    public class SelectorsAccessors
       : BaseAccessors<Selectors, ISelectors>,
       ISelectorsAccessors
    {
        /// <summary>
        /// Initializes a new instance of the SelectorsAccessors class.
        /// The base constructor is called with the relative or absolute file path for the selectors configuration.
        /// </summary>
        public SelectorsAccessors()
            : base(FilePaths.RelativeSelectorsAbsoluteFilePath)
        {
        }
    }
}