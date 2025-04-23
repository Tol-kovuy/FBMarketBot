namespace FBMarketBot.Settings.Selectors.ListingFormSlctrs
{
    /// <summary>
    /// Represents the interface for defining XPath and CSS selectors used in the listing form.
    /// This interface provides a collection of selectors for interacting with various elements in the marketplace listing form.
    /// </summary>
    public class ListingFormSelectors : IListingFormSelectors
    {
        // Selectors for Go to New Listing
        public string HeaderMarketplaceByXPath { get; set; }
        public string ToolBarsMarketplaceByXPath { get; set; }
        public string ExScript { get; set; }
        public string NewListingByXPath { get; set; }
        public string ItemForSaleByXPath { get; set; }
        public string NextNewListingByXPath { get; set; }

        // Video Selectors
        public string ExVideoScript { get; set; }
        public string Complete { get; set; }
        public string VideoByCssSelector { get; set; }
        public string VideoAttribute { get; set; }
        public string StartsWithAttr { get; set; }

        // Image Selectors
        public string ImageByXPath { get; set; }
        public string ExImageScript { get; set; }

        // Title Selectors
        public string TitleBySccSelector { get; set; }
        public string InputByTagName { get; set; }

        // Price Selectors
        public string PriceByCssSelector { get; set; }

        // Category Selectors
        public string CategoryBtnByXPath { get; set; }
        public string NewCategoryBtnByXPath { get; set; }
        public string SelectedCategory { get; set; }
        public string NormalizedSelectedCategory { get; set; }
        public string ChoosesCategoryByXPath { get; set; }
        public string SelectDropDownOption { get; set; }
        public string SelectDropDownOptionClick { get; set; }
        public string TypeInputValue { get; set; }
        public string Colour { get; set; }
        public string Material { get; set; }
        public string DecorStyle { get; set; }
        public string NumberOfPieces { get; set; }
        public string Type { get; set; }
        public string SeatHeight { get; set; }

        // Condition Selectors
        public string ConditionByXPath { get; set; }
        public string ExConditionScript { get; set; }
        public string ChoosesConditionByXPath { get; set; }

        // Description Selectors
        public string DescriptionBySccSelector { get; set; }
        public string TextAreaByTagName { get; set; }


        // Click Next Btn Selectors
        public string NextBtnByXPath { get; set; }

        // Click Publish Btn Selectors
        public string PublishBtnByXPath { get; set; }

        // SubCategories Selectors
        public string BedSizeByXPath { get; set; }
        public string BedSizeOrTypeValue { get; set; }
        public string BedTypeByXPath { get; set; }
        public string ColorByXPath { get; set; }
        public string BrandByXPath { get; set; }

        // CheckBox Selectors
        public string CheckBoxByXPath { get; set; }

        // SKU Selectors
        public string SkuXPath { get; set; }

        // Product Tags Selectors
        public string TagsLabelByCss { get; set; }
        public string PlusBtnByCss { get; set; }
        public string ChooseListAsByXPath { get; set; }
        public string ChooseListValueXPath { get; set; }

        // Location Selectors
        public string LocationByXPath { get; set; }
        public string DropDownLocationsByCss { get; set; }
    }
}
