using FBMarketBot.BrowserAutomation;
using FBMarketBot.GoLogin;
using FBMarketBot.GoLogin.ProfileDTOs;
using FBMarketBot.ListngDataService;
using FBMarketBot.Settings;
using FBMarketBot.Settings.ApplicationSettingsAccessor;
using FBMarketBot.Settings.Categories.SubCategories;
using FBMarketBot.Settings.Schedule;
using FBMarketBot.Settings.Selectors.AccessorSlctrs;
using FBMarketBot.Settings.Selectors.ListingFormSlctrs;
using FBMarketBot.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FBMarketBot.FillingListingForm
{

    /// <summary>
    /// Implements functionality to fill and submit a listing form using browser automation.
    /// </summary>
    public class FormFilling : IFormFilling
    {
        private IList<string> _listingFolders;

        private readonly IBrowserAutomation _page;
        private readonly IApplicationSettingsAccessor _settings;
        private readonly IGoLoginApiService _goLoginApiService;
        private readonly IListingFormSelectors _listingFormSelectors;
        private readonly IListingDataService _listingDataService;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the FormFilling class with required dependencies.
        /// </summary>
        /// <param name="browser">Browser automation service for interacting with the webpage.</param>
        /// <param name="settings">Accessor for application settings.</param>
        /// <param name="selectors">Accessor for form selectors.</param>
        /// <param name="logger">Logger instance for logging operations and errors.</param>
        public FormFilling(
           IBrowserAutomation browser,
           IApplicationSettingsAccessor settings,
           ISelectorsAccessors selectors,
           IGoLoginApiService goLoginApiService,
           ILogger<FormFilling> logger,
           IListingDataService listingDataService
            )
        {
            _page = browser;
            _settings = settings;
            _goLoginApiService = goLoginApiService;
            _listingFormSelectors = selectors.Get().ListingFormSelectors;
            _logger = logger;
            _listingDataService = listingDataService;
        }

        /// <summary>
        /// Asynchronously fills and submits a listing form based on the provided listing number.
        /// </summary>
        /// <param name="listingNumber">The identifier used to fetch listing details such as images, title, and description.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task FillFormByListingNumberAsync(Profile profile)
        {
            await CreateNextListingAsync(profile.NumberListingByProfile);
            await _settings.DelayBetweenActionsAsync();
            await AddImagesAsync(profile.NumberListingByProfile);

            // AddVideoAsync not working in Chromium browser, but working in Chrome browser. 
            // maybe bug from Chromium browser side. Investigate needs.
            //await AddVideoAsync(profile.NumberListingByProfile);

            await AddTitleAsync(profile.NumberListingByProfile);
            await AddPriceAsync();
            await AddCategoryAsync();
            await AddConditionAsync();
            await AddDescriptionAsync(profile.NumberListingByProfile);
            await ChooseListAs();
            await AddSkuAsync();
            await AddLocationAsync(profile.NumberListingByProfile);
            await ClickPublicMeetupAsync();
            await ClickDoorPickupAsync();
            await ClickDoorDropoffAsync();
            await ClickBoostListingAfterPublishAsync();
            await ClickHideFromFriendsAsync();
            await ClickNextBottonAsync();
            await ClickPublishButtonAsync(profile);
            await _settings.DelayBetweenActionsAsync();
        }

        /// <summary>
        /// Asynchronously uploads images to the listing form from a specified folder.
        /// </summary>
        /// <param name="folderName">The name of the folder containing the images.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddImagesAsync(string folderName)
        {
            string photosDirectory = Path.Combine(Directory.GetCurrentDirectory(), FilePaths.PhotosPath, folderName);

            var photoFiles = Directory
                .EnumerateFiles(photosDirectory)
                .OrderBy(file => file)
                .Take(10)
                .ToList();
            await _settings.DelayBetweenActionsAsync();

            foreach (var photoFile in photoFiles)
            {
                try
                {
                    var fileInput = await _page.FindElementAsync(_listingFormSelectors.ImageByXPath);
                    await _settings.DelayBetweenActionsAsync();

                    while (fileInput == null)
                    {
                        fileInput = await _page.WaitForSelectorAsync(_listingFormSelectors.ImageByXPath);
                        await _settings.DelayBetweenActionsAsync();
                        _logger.LogInformation($"Selector {_listingFormSelectors.ImageByXPath} is looking for...");

                        if (fileInput != null)
                        {
                            _logger.LogInformation($"ImageByXPath selector {_listingFormSelectors.ImageByXPath} found!");
                        }
                    }

                    await fileInput.UploadFileAsync(photoFile);
                    _logger.LogInformation($"Image '{photoFile}' was added to your listing");
                    await _settings.DelayBetweenActionsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error uploading image '{photoFile}': {ex.Message}");
                }
            }
            await _settings.DelayBetweenActionsAsync();
        }

        /// <summary>
        /// Asynchronously uploads a video to the listing form based on the provided video name.
        /// </summary>
        /// <param name="videoName">The name of the video file to upload.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddVideoAsync(string videoName)
        {
            string videoPath = Path.Combine(Directory.GetCurrentDirectory(), FilePaths.VideoPath);
            string selectedVideoPath = Directory
                .EnumerateFiles(videoPath)
                .FirstOrDefault(file => Path.GetFileNameWithoutExtension(file) == videoName);

            if (selectedVideoPath == null)
            {
                _logger.LogError($"No video with name '{videoName}'");
                return;
            }

            try
            {
                var fileInputs = await _page.FindAllElementsAsync(_listingFormSelectors.VideoByCssSelector);

                if (!fileInputs.Any())
                {
                    _logger.LogError("No file input elements found on the page.");
                    return;
                }

                await fileInputs.LastOrDefault().UploadFileAsync(selectedVideoPath);
                _logger.LogInformation($"Video '{selectedVideoPath}' has been successfully uploaded.");
                await _settings.DelayBetweenActionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while adding video: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Asynchronously sets the title of the listing from a file based on the listing number.
        /// </summary>
        /// <param name="titleNumber">The index used to select the title from a file.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddTitleAsync(string titleNumber)
        {
            string titlePath = Path.Combine(Directory.GetCurrentDirectory(), FilePaths.TitlePath);

            if (!int.TryParse(titleNumber, out int number))
            {
                _logger.LogError($"Title number '{titleNumber}' is not a valid number.");
                return;
            }

            if (!File.Exists(titlePath))
            {
                _logger.LogError("Title file does not exist.");
                return;
            }

            string[] titles = File.ReadAllLines(titlePath);

            if (titles.Length == 0)
            {
                _logger.LogError("Title file is empty.");
                return;
            }

            if (number < 0 || number >= titles.Length)
            {
                _logger.LogError($"Title number {number} is out of range (0-{titles.Length - 1}).");
                return;
            }

            string title = titles[number];

            try
            {
                await _settings.DelayBetweenActionsAsync();
                var labelElement = await _page.WaitForXPathAsync(_listingFormSelectors.TitleBySccSelector);
                await _settings.DelayBetweenActionsAsync();

                while (labelElement == null)
                {
                    labelElement = await _page.FindElementAsync(_listingFormSelectors.TitleBySccSelector);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {_listingFormSelectors.TitleBySccSelector} is looking for...");

                    if (labelElement != null)
                    {
                        _logger.LogInformation($"TitleBySccSelector selector {_listingFormSelectors.TitleBySccSelector} found!");
                    }
                }

                await labelElement.ClickAsync();
                await labelElement.TypeTextLikeHumanAsync(title);
                _logger.LogInformation($"Title set as '{title}'");
                await _settings.DelayBetweenActionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error while entering title: {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously sets the price of the listing from application settings.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddPriceAsync()
        {
            await _settings.DelayBetweenActionsAsync();

            var priceListing = _settings.Get().ListingFormSettings.Price;

            if (priceListing == default)
            {
                _logger.LogError("Listing price not set! This field required.");
                throw new Exception("Price is not setting.");
            }

            var labelElement = await _page.FindElementAsync(_listingFormSelectors.PriceByCssSelector);

            while (labelElement == null)
            {
                labelElement = await _page.FindElementAsync(_listingFormSelectors.PriceByCssSelector);
                await _settings.DelayBetweenActionsAsync();
                _logger.LogInformation($"Selector {_listingFormSelectors.PriceByCssSelector} is looking for...");

                if (labelElement != null)
                {
                    _logger.LogInformation($"PriceByCssSelector selector {_listingFormSelectors.PriceByCssSelector} found!");
                }
            }

            await labelElement.TypeTextLikeHumanAsync(priceListing);
            _logger.LogInformation($"Price set {priceListing}");
            await _settings.DelayBetweenActionsAsync();
        }

        private async Task AddCategoryAsync()
        {
            var selectedCategory = _settings.Get().ListingFormSettings.Category;
            if (string.IsNullOrEmpty(selectedCategory))
            {
                _logger.LogError("Category not selected! This field is required.");
                return;
            }

            try
            {
                await _settings.DelayBetweenActionsAsync();
                var (categoryButton, isNewCategoryButton) = await GetCategoryButtonAsync();
                if (categoryButton == null)
                {
                    _logger.LogError("Category button not found.");
                    return;
                }

                await categoryButton.ClickAsync();
                await _settings.DelayBetweenActionsAsync();

                var selector = GetCategorySelector(selectedCategory);

                var categoryElement = await WaitForElementAsync(selector);

                var text = await categoryElement.EvaluateFunctionAsync<string>("el => el.innerText");

                int attempts = 0;
                const int maxAttempts = 5;
                while (!text.Equals(selectedCategory, StringComparison.CurrentCultureIgnoreCase) && attempts < maxAttempts)
                {
                    attempts++;
                    _logger.LogInformation($"Attempt {attempts}: Text '{text}' != Expected '{selectedCategory}'");

                    await _settings.DelayBetweenActionsAsync();
                    categoryElement = await WaitForElementAsync(selector);
                    text = await categoryElement.EvaluateFunctionAsync<string>("el => el.innerText.trim()");
                }

                if (attempts >= maxAttempts)
                {
                    _logger.LogError($"Failed to match text after {maxAttempts} attempts");
                    return;
                }

                _logger.LogInformation($"For testing: innerText = {text}");
                await _settings.DelayBetweenActionsAsync();

                if (categoryElement == null)
                {
                    _logger.LogError($"Category selector '{selector}' not found.");
                    return;
                }

                await categoryElement.ClickAsync();
                _logger.LogInformation($"Category '{selectedCategory}' was selected.");
                await _settings.DelayBetweenActionsAsync();

                if (isNewCategoryButton)
                {
                    var subCategory = await SelectSubCategoriesAsync();
                    await SetSubCategoryFields(subCategory);
                }
                else
                {
                    await AddlFieldsForCategoryAsync();
                    await AddProductTagsAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error while selecting category '{selectedCategory}': {ex.Message}");
            }
        }

        private async Task<(IPageElement Button, bool IsNewCategory)> GetCategoryButtonAsync()
        {
            var categoryButton = await _page.FindElementAsync(_listingFormSelectors.NewCategoryBtnByXPath);
            if (categoryButton != null)
            {
                return (categoryButton, true);
            }

            categoryButton = await _page.FindElementAsync(_listingFormSelectors.CategoryBtnByXPath);
            if (categoryButton != null)
            {
                _logger.LogInformation($"Using old category button: {_listingFormSelectors.CategoryBtnByXPath}");
            }

            return (categoryButton, false);
        }

        private string GetCategorySelector(string selectedCategory)
        {
            if (!selectedCategory.Contains("&"))
                return string.Format(_listingFormSelectors.SelectedCategory, selectedCategory);

            var normalizedCategory1 = NormalizeCategory(selectedCategory.Replace("&", "and"));
            var normalizedCategory2 = selectedCategory.Replace("and", "&");
            return string.Format(_listingFormSelectors.NormalizedSelectedCategory, normalizedCategory1, normalizedCategory2);
        }

        private string NormalizeCategory(string category)
        {
            var words = category.Split(' ');
            return string.Join(" ", words.Select((word, index) => index == 0 ? word : word.ToLower()));
        }

        private async Task<IPageElement> WaitForElementAsync(string selector)
        {
            var element = await _page.WaitForXPathAsync(selector);
            if (element != null)
            {
                return element;
            }

            _logger.LogInformation($"Waiting for selector: {selector}...");
            await _settings.DelayBetweenActionsAsync();
            element = await _page.WaitForXPathAsync(selector);
            return element;
        }

        private async Task<string> SelectSubCategoriesAsync()
        {
            var subCategories = _settings.Get().ListingFormSettings.SubCategories;
            if (!subCategories.Any())
            {
                _logger.LogError("SubCategory not selected! This field is required.");
            }

            foreach (var subCategory in subCategories)
            {
                var subSelector = string.Format(_listingFormSelectors.SelectedCategory, subCategory);
                var element = await WaitForElementAsync(subSelector);
                if (element == null)
                {
                    _logger.LogError($"SubCategory '{subCategory}' not found.");
                    continue;
                }

                await element.ClickAsync();
                _logger.LogInformation($"SubCategory '{subCategory}' was selected.");
                await _settings.DelayBetweenActionsAsync();
            }

            return subCategories.LastOrDefault();
        }

        private async Task AddConditionAsync()
        {
            var condition = _settings.Get().ListingFormSettings.Condition;

            if (string.IsNullOrEmpty(condition))
            {
                _logger.LogError("Condition not selected! This field required.");
                return;
            }

            try
            {
                await _settings.DelayBetweenActionsAsync();
                var conditionButton = await _page.FindElementAsync(_listingFormSelectors.ConditionByXPath);

                while (conditionButton == null)
                {
                    conditionButton = await _page.FindElementAsync(_listingFormSelectors.ConditionByXPath);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {_listingFormSelectors.ConditionByXPath} is looking for...");

                    if (conditionButton != null)
                    {
                        _logger.LogInformation($"ConditionByXPath selector {_listingFormSelectors.ConditionByXPath} found!");
                    }
                }

                await conditionButton.EvaluateFunctionAsync<string>("el => el.className");
                await conditionButton.ClickAsync();
                _logger.LogInformation("Clicked on the 'Condition' button successfully.");
                await _settings.DelayBetweenActionsAsync();

                var xpath = string.Format(_listingFormSelectors.ChoosesConditionByXPath, condition);
                var newOption = await _page.WaitForXPathAsync(xpath);

                while (newOption == null)
                {
                    newOption = await _page.WaitForXPathAsync(xpath);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {xpath} is looking for...");

                    if (newOption != null)
                    {
                        _logger.LogInformation($"ChoosesConditionByXPath selector {xpath} found!");
                    }
                }

                await _settings.DelayBetweenActionsAsync();
                await newOption.ClickAsync();

                _logger.LogInformation($"Condition was selected as {condition}.");
                await _settings.DelayBetweenActionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error while clicking on the 'Condition' button. " + ex.Message);
            }
        }

        #region for review
        private ISubCategoryFields GetFields()
        {
            return _settings.Get()
                .ListingFormSettings
                .SubCategoryFields;
        }

        private async Task SetSubCategoryFields(string subCategory)
        {
            var formFields = new List<FormField>();

            if (string.Equals(subCategory, FilePaths.OtherArtSupplies, StringComparison.InvariantCultureIgnoreCase))
            {
                var colourField = GetFields().Colour_For_ArtSupplies_BarStools_DiningChairs_OtherLivingRoom;
                await SelectDropdownOptionAsync(colourField, _listingFormSelectors.Colour);

                var materialField = GetFields().Material_For_ArtSupplies_BarStools_OtherLivingRoom;
                await TypeInputValueAsync(_listingFormSelectors.Material, materialField);
            }

            if (string.Equals(subCategory, FilePaths.BarStools, StringComparison.InvariantCultureIgnoreCase))
            {
                formFields.AddRange(new[]
                {
                    new FormField(GetFields().Colour_For_ArtSupplies_BarStools_DiningChairs_OtherLivingRoom, _listingFormSelectors.Colour),
                    new FormField(GetFields().Material_For_ArtSupplies_BarStools_OtherLivingRoom, _listingFormSelectors.Material),
                    new FormField(GetFields().SeatHeight_For_BarStools, _listingFormSelectors.SeatHeight)
                });

                foreach (var fields in formFields)
                {
                    await SelectDropdownOptionAsync(fields.Value, fields.Label);
                }
            }

            if (string.Equals(subCategory, FilePaths.DiningChairs, StringComparison.InvariantCultureIgnoreCase))
            {
                formFields.AddRange(new[]
                {
                    new FormField(GetFields().Type_For_DiningChairs, _listingFormSelectors.Type),
                    new FormField(GetFields().Colour_For_ArtSupplies_BarStools_DiningChairs_OtherLivingRoom, _listingFormSelectors.Colour),
                    new FormField(GetFields().NumberOfPieces_For_DiningChairs, _listingFormSelectors.NumberOfPieces),
                    new FormField(GetFields().DecorStyle_For_DiningChairs_OtherLivingRoom, _listingFormSelectors.DecorStyle)
                });

                foreach (var fields in formFields)
                {
                    await SelectDropdownOptionAsync(fields.Value, fields.Label);
                }
            }

            if (string.Equals(subCategory, FilePaths.OtherLivingRoomFurniture, StringComparison.InvariantCultureIgnoreCase))
            {
                formFields.AddRange(new[]
                {
                    new FormField(GetFields().Colour_For_ArtSupplies_BarStools_DiningChairs_OtherLivingRoom, _listingFormSelectors.Colour),
                    new FormField(GetFields().DecorStyle_For_DiningChairs_OtherLivingRoom, _listingFormSelectors.DecorStyle),
                    new FormField(GetFields().Material_For_ArtSupplies_BarStools_OtherLivingRoom, _listingFormSelectors.Material)
                });

                foreach (var fields in formFields)
                {
                    await SelectDropdownOptionAsync(fields.Value, fields.Label);
                }
            }
        }

        private async Task SelectDropdownOptionAsync(string value, string label)
        {
            try
            {
                await _SelectDropdownOptionAsync(_listingFormSelectors.SelectDropDownOption, label);
                await _SelectDropdownOptionAsync(_listingFormSelectors.SelectDropDownOptionClick, value);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error selecting '{label}' - '{value}': {ex.Message}");
            }
        }

        private async Task _SelectDropdownOptionAsync(string format, string argument)
        {
            var selector = string.Format(format, argument);
            var button = await _page.WaitForXPathAsync(selector);
            await _settings.DelayBetweenActionsAsync();
            await button.ClickAsync();
            _logger.LogInformation($"'{argument}' button was clicked.");
            await _settings.DelayBetweenActionsAsync();
        }

        private async Task TypeInputValueAsync(string label, string value)
        {
            try
            {
                var selector = string.Format(_listingFormSelectors.TypeInputValue, label);
                var inputField = await _page.WaitForXPathAsync(selector);
                await _settings.DelayBetweenActionsAsync();
                await inputField.TypeTextLikeHumanAsync(value);
                _logger.LogInformation($"In input field, the text '{value}' is entered.");
                await _settings.DelayBetweenActionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error typing '{value}': {ex.Message}");
            }
        }
        #endregion

        /// <summary>
        /// Asynchronously adds a description to the listing from a file based on the listing number.
        /// </summary>
        /// <param name="descriptionNumber">The index used to select the description from a file.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddDescriptionAsync(string descriptionNumber)
        {
            var descriptionsPath = Path.Combine(Directory.GetCurrentDirectory(), FilePaths.DescriptionsPath);
            var intTitleNumber = int.TryParse(descriptionNumber, out int number);

            if (!intTitleNumber)
            {
                _logger.LogError("Description number not correct.");
                return;
            }

            if (!File.Exists(descriptionsPath))
            {
                _logger.LogError($"Description file missing {descriptionsPath}");
                return;
            }

            string[] descriptions = File.ReadAllLines(descriptionsPath);
            var description = descriptions[number];

            try
            {
                await _settings.DelayBetweenActionsAsync();
                var descriptionContainer = await _page.FindElementAsync(_listingFormSelectors.DescriptionBySccSelector);

                while (descriptionContainer == null)
                {
                    descriptionContainer = await _page.FindElementAsync(_listingFormSelectors.DescriptionBySccSelector);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Description selector {_listingFormSelectors.DescriptionBySccSelector} is looking for...");

                    if (descriptionContainer != null)
                    {
                        _logger.LogInformation($"DescriptionBySccSelector selector {_listingFormSelectors.DescriptionBySccSelector} found!");
                    }
                }

                await _settings.DelayBetweenActionsAsync();
                var descriptionTextarea = await descriptionContainer.FindElementAsync(_listingFormSelectors.TextAreaByTagName);

                while (descriptionTextarea == null)
                {
                    descriptionTextarea = await descriptionContainer.FindElementAsync(_listingFormSelectors.TextAreaByTagName);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Description_TextAreaByTagName selector {_listingFormSelectors.TextAreaByTagName} is looking for...");

                    if (descriptionTextarea != null)
                    {
                        _logger.LogInformation($"Description_TextAreaByTagName selector {_listingFormSelectors.TextAreaByTagName} found!");
                    }
                }

                await descriptionTextarea.TypeTextLikeHumanAsync(description);

                _logger.LogInformation($"Description set as {description}. For test listing - {descriptionNumber}, descr - {description}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error while entering description. " + ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously adds additional fields specific to the selected category.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddlFieldsForCategoryAsync()
        {
            var listingSettings = _settings.Get().ListingFormSettings;
            var selectedCategory = listingSettings.Category;

            if (selectedCategory == FilePaths.FurnitureCategory)
            {
                await AddBedSize(listingSettings.BedSize);
                await AddBedType(listingSettings.BedType);
                await AddColor(listingSettings.Color);
                await AddBrand(listingSettings.Brand);
                return;
            }

            if (selectedCategory == FilePaths.HouseholdCategory)
            {
                await AddColor(listingSettings.Color);
                return;
            }

            if (selectedCategory == FilePaths.ArtsAndCraftsCategory)
            {
                await AddBrand(listingSettings.Brand);
                return;
            }
        }

        /// <summary>
        /// Asynchronously adds the bed size to the listing form.
        /// </summary>
        /// <param name="value">The bed size value to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddBedSize(string value)
        {
            await AddDropdownValue(_listingFormSelectors.BedSizeByXPath, value);
            _logger.LogInformation($"Bed Size '{value}' was added to the input field.");
        }

        /// <summary>
        /// Asynchronously adds the bed type to the listing form.
        /// </summary>
        /// <param name="value">The bed type value to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddBedType(string value)
        {
            await AddDropdownValue(_listingFormSelectors.BedTypeByXPath, value);
            _logger.LogInformation($"Bed Type '{value}' was added to the input field.");
        }

        /// <summary>
        /// Asynchronously adds the color to the listing form.
        /// </summary>
        /// <param name="value">The color value to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddColor(string value)
        {
            await AddInputValue(_listingFormSelectors.ColorByXPath, value);
            _logger.LogInformation($"Color '{value}' was added to the input field.");
        }

        /// <summary>
        /// Asynchronously adds the brand to the listing form.
        /// </summary>
        /// <param name="value">The brand value to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddBrand(string value)
        {
            await AddInputValue(_listingFormSelectors.BrandByXPath, value);
            _logger.LogInformation($"Brand '{value}' was added to the input field.");
        }

        /// <summary>
        /// Asynchronously selects a value from a dropdown menu in the listing form.
        /// </summary>
        /// <param name="comboboxXPath">The XPath of the dropdown element.</param>
        /// <param name="value">The value to select from the dropdown.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddDropdownValue(string comboboxXPath, string value)
        {
            try
            {
                var combobox = await _page.WaitForXPathAsync(comboboxXPath);

                while (combobox == null)
                {
                    combobox = await _page.WaitForXPathAsync(comboboxXPath);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {comboboxXPath} is looking for...");

                    if (combobox != null)
                    {
                        _logger.LogInformation($"ComboboxXPath selector {comboboxXPath} found!");
                    }
                }

                await combobox.ClickAsync();

                await _settings.DelayBetweenActionsAsync();

                var xpath = string.Format(_listingFormSelectors.BedSizeOrTypeValue, value);
                var listItem = await _page.WaitForXPathAsync(xpath);

                while (listItem == null)
                {
                    listItem = await _page.WaitForXPathAsync(xpath);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {xpath} is looking for...");

                    if (listItem != null)
                    {
                        _logger.LogInformation($"BedSizeOrTypeValue selector {xpath} found!");
                    }
                }

                await listItem.ClickAsync();

                await _settings.DelayBetweenActionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously enters a value into an input field in the listing form.
        /// </summary>
        /// <param name="inputXPath">The XPath of the input field.</param>
        /// <param name="value">The value to enter into the input field.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddInputValue(string inputXPath, string value)
        {
            try
            {
                var inputField = await _page.WaitForXPathAsync(inputXPath);

                while (inputField == null)
                {
                    inputField = await _page.WaitForXPathAsync(inputXPath);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {inputXPath} is looking for...");

                    if (inputField != null)
                    {
                        _logger.LogInformation($"InputXPath selector {inputXPath} found!");
                    }
                }

                await inputField.TypeTextLikeHumanAsync(value);

                await _settings.DelayBetweenActionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously selects the "List As" option from a dropdown in the listing form.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ChooseListAs()
        {
            var value = _settings.Get().ListingFormSettings.ListAsSingleItem;
            if (string.IsNullOrEmpty(value))
            {
                _logger.LogWarning("ListAsSingleItem value is default. Skipping this step.");
                return;
            }

            var dropdownSelector = _listingFormSelectors.ChooseListAsByXPath;
            var dropdownValue = _listingFormSelectors.ChooseListValueXPath;

            try
            {
                var dropdown = await _page.WaitForXPathAsync(dropdownSelector);

                while (dropdown == null)
                {
                    dropdown = await _page.WaitForXPathAsync(dropdownSelector);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {dropdownSelector} is looking for...");

                    if (dropdown != null)
                    {
                        _logger.LogInformation($"DropdownSelector selector {dropdownSelector} found!");
                    }
                }

                await dropdown.ClickAsync();

                var optionXPath = string.Format(dropdownValue, value);
                var option = await _page.WaitForXPathAsync(optionXPath);

                while (option == null)
                {
                    option = await _page.WaitForXPathAsync(optionXPath);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {optionXPath} is looking for...");

                    if (option != null)
                    {
                        _logger.LogInformation($"OptionXPath selector {optionXPath} found!");
                    }
                }

                await option.ClickAsync();
                await _settings.DelayBetweenActionsAsync();

                _logger.LogInformation($"Value '{value}' was successfully selected from the 'List As' dropdown.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error while selecting value '{value}' from the 'List As' dropdown. {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously adds product tags to the listing form from application settings.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddProductTagsAsync()
        {
            var productTags = _settings.Get().ListingFormSettings.ProductTags;

            if (productTags == null || !productTags.Any())
            {
                _logger.LogInformation("No tags chosen.");
                return;
            }

            var limitedTags = productTags.Take(20).ToList();
            if (!limitedTags.Any())
            {
                _logger.LogInformation("No valid tags to add.");
                return;
            }

            foreach (var tag in limitedTags)
            {
                _logger.LogInformation($"Processing tag: {tag}");
                await AddTagAsync(tag);
            }
        }

        /// <summary>
        /// Asynchronously adds a single tag to the listing form.
        /// </summary>
        /// <param name="tag">The tag to add to the listing.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddTagAsync(string tag)
        {
            var tagSelector = _listingFormSelectors.TagsLabelByCss;
            var plusBtn = _listingFormSelectors.PlusBtnByCss;

            try
            {
                var productTagsLabel = await _page.WaitForXPathAsync(tagSelector);

                while (productTagsLabel == null)
                {
                    productTagsLabel = await _page.WaitForXPathAsync(tagSelector);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {tagSelector} is looking for...");

                    if (productTagsLabel != null)
                    {
                        _logger.LogInformation($"TagSelector selector {tagSelector} found!");
                    }
                }

                await productTagsLabel.TypeTextLikeHumanAsync(tag);

                var plusButton = await _page.FindElementAsync(plusBtn);

                while (plusButton == null)
                {
                    plusButton = await _page.FindElementAsync(plusBtn);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {plusButton} is looking for...");

                    if (plusButton != null)
                    {
                        _logger.LogInformation($"PlusButton selector {plusButton} found!");
                    }
                }

                await plusButton.ClickAsync();
                await _settings.DelayBetweenActionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while adding tag: {tag}. Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously adds the SKU (Stock Keeping Unit) to the listing form from application settings.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddSkuAsync()
        {
            var sku = _settings.Get().ListingFormSettings.SKU;

            if (string.IsNullOrEmpty(sku))
            {
                return;
            }

            var selector = _listingFormSelectors.SkuXPath;

            try
            {
                var skuInput = await _page.WaitForXPathAsync(selector);

                while (skuInput == null)
                {
                    skuInput = await _page.WaitForXPathAsync(selector);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {selector} is looking for...");

                    if (skuInput != null)
                    {
                        _logger.LogInformation($"SkuInput selector {selector} found!");
                    }
                }

                await skuInput.TypeTextLikeHumanAsync(sku);

                _logger.LogInformation($"SKU {sku} was added.");
                await _settings.DelayBetweenActionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously adds a location to the listing form from a file based on the provided location number.
        /// </summary>
        /// <param name="locationNumber">The index used to select the location from a file.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddLocationAsync(string locationNumber)
        {
            string locationPath = Path.Combine(Directory.GetCurrentDirectory(), FilePaths.LocationPath);
            var intLocationNumber = int.TryParse(locationNumber, out int number);

            if (!intLocationNumber)
            {
                _logger.LogError("Location number not correct: {LocationNumber}", locationNumber);
                return;
            }

            string location = null;

            if (File.Exists(locationPath))
            {
                string[] locations = File.ReadAllLines(locationPath);
                if (number < 0 || number >= locations.Length)
                {
                    _logger.LogError("Location number {LocationNumber} is out of range. Max index: {MaxIndex}", number, locations.Length - 1);
                    return;
                }
                location = locations[number];
            }

            try
            {
                var inputElement = await _page.WaitForXPathAsync(_listingFormSelectors.LocationByXPath);

                while (inputElement == null)
                {
                    inputElement = await _page.WaitForXPathAsync(_listingFormSelectors.LocationByXPath);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {_listingFormSelectors.LocationByXPath} is looking for...");

                    if (inputElement != null)
                    {
                        _logger.LogInformation($"LocationByXPath selector {_listingFormSelectors.LocationByXPath} found!");
                    }
                }

                await _settings.DelayBetweenActionsAsync();
                await inputElement.ClickAsync();
                await inputElement.TypeTextLikeHumanAsync(location);

                await _settings.DelayBetweenActionsAsync();

                var element = await _page.FindElementAsync(_listingFormSelectors.DropDownLocationsByCss);

                while (element == null)
                {
                    element = await _page.FindElementAsync(_listingFormSelectors.DropDownLocationsByCss);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {_listingFormSelectors.DropDownLocationsByCss} is looking for...");

                    if (element != null)
                    {
                        _logger.LogInformation($"DropDownLocationsByCss selector {_listingFormSelectors.DropDownLocationsByCss} found!");
                    }
                }

                await _settings.DelayBetweenActionsAsync();
                await element.ClickAsync();
                _logger.LogInformation($"Location {location} was selected.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously enables the public meetup option if specified in settings.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ClickPublicMeetupAsync()
        {
            var isPublicMeetup = _settings.Get().ListingFormSettings.PublicMeetUp;
            if (isPublicMeetup)
            {
                await ClickCheckboxAsync(FilePaths.PublicMeetUp, "Public meetup was clicked.");
            }
        }

        /// <summary>
        /// Asynchronously enables the door pickup option if specified in settings.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ClickDoorPickupAsync()
        {
            var isDoorPickup = _settings.Get().ListingFormSettings.DoorPickUp;
            if (isDoorPickup)
            {
                await ClickCheckboxAsync(FilePaths.DoorPickup, "Door pickup was clicked.");
            }
        }

        /// <summary>
        /// Asynchronously enables the door drop-off option if specified in settings.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ClickDoorDropoffAsync()
        {
            var isDoorDropOff = _settings.Get().ListingFormSettings.DoorDropOff;
            if (isDoorDropOff)
            {
                await ClickCheckboxAsync(FilePaths.DoorDropOff, "Door dropoff was clicked.");
            }
        }

        /// <summary>
        /// Asynchronously enables the boost listing option if specified in settings.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ClickBoostListingAfterPublishAsync()
        {
            var isBoostListing = _settings.Get().ListingFormSettings.BoostListingAfterPublish;
            if (isBoostListing)
            {
                await ClickCheckboxAsync(FilePaths.BoostListingAfterPublish, "Boost listing after publish was clicked.");
            }
        }

        /// <summary>
        /// Asynchronously enables the hide from friends option if specified in settings.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ClickHideFromFriendsAsync()
        {
            var isHideFromFriends = _settings.Get().ListingFormSettings.HideFromFriends;
            if (isHideFromFriends)
            {
                await ClickCheckboxAsync(FilePaths.HideFromFriends, "Hide from friends was clicked.");
            }
        }

        /// <summary>
        /// Asynchronously clicks a checkbox in the listing form based on the provided file path.
        /// </summary>
        /// <param name="filePath">The file path used to construct the checkbox XPath.</param>
        /// <param name="logMessage">The message to log upon successful click.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ClickCheckboxAsync(string filePath, string logMessage)
        {
            try
            {
                var selector = _listingFormSelectors.CheckBoxByXPath;
                var xpath = string.Format(selector, filePath);
                var element = await _page.WaitForXPathAsync(xpath);

                while (element == null)
                {
                    element = await _page.WaitForXPathAsync(xpath);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {xpath} is looking for...");

                    if (element != null)
                    {
                        _logger.LogInformation($"ClickCheckbox selector {xpath} found!");
                    }
                }

                await element.ClickAsync();

                _logger.LogInformation(logMessage);
                await _settings.DelayBetweenActionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously clicks the "Next" button in the listing form.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ClickNextBottonAsync()
        {
            try
            {
                await _settings.DelayBetweenActionsAsync();
                var nextButton = await _page.WaitForXPathAsync(_listingFormSelectors.NextBtnByXPath);

                while (nextButton == null)
                {
                    nextButton = await _page.WaitForXPathAsync(_listingFormSelectors.NextBtnByXPath);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {_listingFormSelectors.NextBtnByXPath} is looking for...");

                    if (nextButton != null)
                    {
                        _logger.LogInformation($"NextBtnByXPath selector {_listingFormSelectors.NextBtnByXPath} found!");
                    }
                }

                await nextButton.ClickAsync();

                _logger.LogInformation("Clicked the 'Next' button successfully.");
                await _settings.DelayBetweenActionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred while clicking the 'Next' button. " + ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously clicks the "Publish" button to submit the listing.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ClickPublishButtonAsync(Profile profile)
        {
            try
            {
                var publishButton = await _page.WaitForXPathAsync(_listingFormSelectors.PublishBtnByXPath);

                while (publishButton == null)
                {
                    publishButton = await _page.WaitForXPathAsync(_listingFormSelectors.PublishBtnByXPath);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {_listingFormSelectors.PublishBtnByXPath} is looking for...");

                    if (publishButton != null)
                    {
                        _logger.LogInformation($"PublishBtnByXPath selector {_listingFormSelectors.PublishBtnByXPath} found!");
                    }
                }

                await _settings.DelayBetweenActionsAsync();
                await publishButton.ClickAsync();

                _logger.LogInformation("Click Publish Button");

                _listingDataService.SavePostedListingByProfile(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred while clicking the 'Publish' button. " + ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously navigates to and initiates the creation of a new listing.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task CreateNextListingAsync(string listingNumber)
        {
            _logger.LogInformation($"\nRandom listing {listingNumber} start posting from Content folder...\n");

            try
            {
                // Open new tab on anti-bots page for testing
                //await _page.OpenNewPageAsync("https://2ip.io//"); // https://whoer.net/ru https://pixelscan.net/ https://bot.sannysoft.com/ https://amiunique.org/fingerprint

                await _page.GoToAsync(FilePaths.NewListingPage);
                await _settings.DelayBetweenActionsAsync();
                var curentPage = await _page.GetCurrentUrl();
                _logger.LogInformation($"Current page now - {curentPage}");

                var button = await _page.FindElementAsync(_listingFormSelectors.NewListingByXPath);

                while (button == null)
                {
                    button = await _page.FindElementAsync(_listingFormSelectors.NewListingByXPath);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {_listingFormSelectors.NewListingByXPath} is looking for...");

                    if (button != null)
                    {
                        _logger.LogInformation($"NewListingByXPath selector {_listingFormSelectors.NewListingByXPath} found!");
                    }
                }

                if (button == null)
                {
                    _logger.LogError("Failed to find the listing button after multiple attempts.");
                    throw new InvalidOperationException("Listing button was not found. Cannot proceed.");
                }

                await button.ClickAsync();
                _logger.LogInformation("Click 'Create New Listing'.");
                await _settings.DelayBetweenActionsAsync();

                var itemForSaleButton = await _page.FindElementAsync(_listingFormSelectors.ItemForSaleByXPath);

                while (itemForSaleButton == null)
                {
                    itemForSaleButton = await _page.FindElementAsync(_listingFormSelectors.ItemForSaleByXPath);
                    await _settings.DelayBetweenActionsAsync();
                    _logger.LogInformation($"Selector {_listingFormSelectors.ItemForSaleByXPath} is looking for...");

                    if (itemForSaleButton != null)
                    {
                        _logger.LogInformation($"ItemForSaleByXPath selector {_listingFormSelectors.ItemForSaleByXPath} found!");
                    }
                }

                await itemForSaleButton.ClickAsync();
                _logger.LogInformation("Click 'Item For Sale'.");
                await _settings.DelayBetweenActionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously publishes listings according to a weekly schedule.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task PublishListingAsync(Profile profile)
        {
            var listingNumbers = _listingDataService.GetUnpostedListings(profile.Id);

            if (listingNumbers == null || listingNumbers.Count == 0)
            {
                _logger.LogInformation("There are no unpublished listings.");
                return;
            }

            var random = new Random();
            var randomIndex = random.Next(0, listingNumbers.Count);
            var selectedListing = listingNumbers[randomIndex];
            profile.NumberListingByProfile = selectedListing;
            await FillFormByListingNumberAsync(profile);

            _logger.LogInformation($"\nListing '{selectedListing}' for '{DateTime.Now.DayOfWeek}' was posted by profile '{profile.Name}'.\n");
            
        }
    }
}
