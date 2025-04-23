using System;

namespace FBMarketBot.Shared
{
    /// <summary>
    /// Provides static file paths and constants used throughout the application.
    /// </summary>
    public static class FilePaths
    {
        /// <summary>
        /// Gets the URL of the Facebook main page.
        /// </summary>
        public static string MainPageUrl { get; } = "https://www.facebook.com/";

        /// <summary>
        /// Gets the URL of the Facebook Marketplace selling page for creating new listings.
        /// </summary>
        public static string NewListingPage { get; } = "https://www.facebook.com/marketplace/you/selling";

        /// <summary>
        /// Gets the relative file path to the application settings JSON file.
        /// </summary>
        public static string RelativeSettingsFilePath { get; } = @"ApplicationSettings\\_appStaticSettings.json";

        /// <summary>
        /// Gets the relative directory path for the user’s browser profile.
        /// </summary>
        public static string RelativeUserBrowserProfilePath { get; } = @"UserProfile";

        /// <summary>
        /// Gets the relative file path to the Facebook selectors JSON file.
        /// </summary>
        public static string RelativeSelectorsAbsoluteFilePath { get; } = @"ApplicationSettings\\_fbSelectors.json";

        /// <summary>
        /// Gets the relative directory path for storing listing photos.
        /// </summary>
        public static string PhotosPath { get; } = @"Content\Photos";

        /// <summary>
        /// Gets the relative directory path for storing listing videos.
        /// </summary>
        public static string VideoPath { get; } = @"Content\Videos";

        /// <summary>
        /// Gets the relative file path to the titles text file for listings.
        /// </summary>
        public static string TitlePath { get; } = @"Content\Titles\_titles.txt";

        /// <summary>
        /// Gets the relative file path to the descriptions text file for listings.
        /// </summary>
        public static string DescriptionsPath { get; } = @"Content\Descriptions\_descriptions.txt";

        /// <summary>
        /// Gets the relative file path to the locations text file for listings.
        /// </summary>
        public static string LocationPath { get; } = @"Content\Locations\_locations.txt";

        /// <summary>
        /// Gets the category name for furniture listings.
        /// </summary>
        public static string FurnitureCategory { get; } = "Furniture";

        /// <summary>
        /// Gets the category name for household listings.
        /// </summary>
        public static string HouseholdCategory { get; } = "Household";

        /// <summary>
        /// Gets the category name for arts and crafts listings.
        /// </summary>
        public static string ArtsAndCraftsCategory { get; } = "Arts & Crafts";

        /// <summary>
        /// Gets the label for the public meetup delivery option.
        /// </summary>
        public static string PublicMeetUp { get; } = "Public meetup";

        /// <summary>
        /// Gets the label for the door pickup delivery option.
        /// </summary>
        public static string DoorPickup { get; } = "Door pickup";

        /// <summary>
        /// Gets the label for the door drop-off delivery option.
        /// </summary>
        public static string DoorDropOff { get; } = "Door dropoff";

        /// <summary>
        /// Gets the label for the boost listing after publish option.
        /// </summary>
        public static string BoostListingAfterPublish { get; } = "Boost listing after publish";

        /// <summary>
        /// Gets the label for the hide from friends option.
        /// </summary>
        public static string HideFromFriends { get; } = "Hide from friends";

        public static string PostedListingPath { get; } = @"PostedListing\";

        public static string OtherArtSupplies { get; } = "Other Art supplies";
        public static string BarStools { get; } = "Bar stools";
        public static string DiningChairs { get; } = "Dining chairs";
        public static string OtherLivingRoomFurniture { get; } = "Other Living room furniture";
    }
}
