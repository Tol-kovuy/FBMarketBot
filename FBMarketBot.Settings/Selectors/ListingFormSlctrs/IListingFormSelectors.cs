namespace FBMarketBot.Settings.Selectors.ListingFormSlctrs
{
    /// <summary>
    /// Represents the interface for defining XPath and CSS selectors used in the listing form.
    /// This interface provides a collection of selectors for interacting with various elements in the marketplace listing form.
    /// </summary>
    public interface IListingFormSelectors
    {
        string CategoryBtnByXPath { get; }
        string NewCategoryBtnByXPath { get; }
        string SelectedCategory { get; }
        string NormalizedSelectedCategory { get; }
        string ChoosesCategoryByXPath { get; }
        string ChoosesConditionByXPath { get; }
        string Complete { get; }
        string ConditionByXPath { get; }
        string DescriptionBySccSelector { get; }
        string ExVideoScript { get; }
        string HeaderMarketplaceByXPath { get; }
        string ImageByXPath { get; }
        string ItemForSaleByXPath { get; }
        string NewListingByXPath { get; }
        string NextBtnByXPath { get; }
        string NextNewListingByXPath { get; }
        string PriceByCssSelector { get; }
        string PublishBtnByXPath { get; }
        string StartsWithAttr { get; }
        string TextAreaByTagName { get; }
        string TitleBySccSelector { get; }
        string ToolBarsMarketplaceByXPath { get; }
        string VideoAttribute { get; }
        string VideoByCssSelector { get; }
        string BedSizeByXPath { get; }
        string BedSizeOrTypeValue { get; }
        string BedTypeByXPath { get; }
        string ColorByXPath { get; }
        string BrandByXPath { get; }
        string CheckBoxByXPath { get; }
        string SkuXPath { get; }
        string TagsLabelByCss { get; }
        string PlusBtnByCss { get; }
        string ChooseListAsByXPath { get; }
        string ChooseListValueXPath { get; }
        string LocationByXPath { get; }
        string DropDownLocationsByCss { get; }
        string SelectDropDownOption { get; }
        string SelectDropDownOptionClick { get; }
        string TypeInputValue { get; }
        string Colour { get; }
        string Material { get; }
        string DecorStyle { get; }
        string NumberOfPieces { get; }
        string Type { get; }
        string SeatHeight { get; }
    }
}
