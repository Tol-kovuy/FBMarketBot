using FBMarketBot.Settings.Categories.SubCategories;
using System.Collections.Generic;

namespace FBMarketBot.Settings.Categories
{
    /// <summary>
    /// Represents settings for configuring a listing form, implementing the IListingFormSettings interface.
    /// </summary>
    public class ListingFormSettings : IListingFormSettings
    {
        public string Price { get; set; }

        public string Category { get; set; }

        public IList<string> SubCategories { get; set; }
        public SubCategoryFields SubCategoryFields { get; set; }

        public string Condition { get; set; }

        public string BedSize { get; set; }

        public string BedType { get; set; }

        public string Color { get; set; }

        public string Brand { get; set; }

        public string ListAsSingleItem { get; set; }

        public IList<string> ProductTags { get; set; }

        public string SKU { get; set; }

        public bool PublicMeetUp { get; set; }

        public bool DoorPickUp { get; set; }

        public bool DoorDropOff { get; set; }

        public bool BoostListingAfterPublish { get; set; }

        public bool HideFromFriends { get; set; }
    }
}