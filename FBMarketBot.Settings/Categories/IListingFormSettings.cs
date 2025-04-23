using FBMarketBot.Settings.Categories.SubCategories;
using System.Collections.Generic;

namespace FBMarketBot.Settings.Categories
{
    /// <summary>
    /// Defines a contract for settings related to configuring a listing form.
    /// </summary>
    public interface IListingFormSettings
    {
        string Price { get; }
        string Category { get; }
        IList<string> SubCategories { get; }
        SubCategoryFields SubCategoryFields { get; }
        string Condition { get; }
        string BedSize { get; }
        string BedType { get; }
        string Color { get; }
        string Brand { get; }
        string ListAsSingleItem { get; }
        IList<string> ProductTags { get; }
        string SKU { get; }
        bool PublicMeetUp { get; }
        bool DoorPickUp { get; }
        bool DoorDropOff { get; }
        bool BoostListingAfterPublish { get; }
        bool HideFromFriends { get; }
    }
}
