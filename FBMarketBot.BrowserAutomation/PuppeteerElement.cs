using PuppeteerSharp;
using System.Threading.Tasks;

namespace FBMarketBot.BrowserAutomation
{
    /// <summary>
    /// Represents a Puppeteer element and provides methods to interact with it asynchronously.
    /// </summary>
    public class PuppeteerElement : IPageElement
    {
        private readonly IElementHandle _elementHandle;

        /// <summary>
        /// Initializes a new instance of the PuppeteerElement class.
        /// </summary>
        /// <param name="elementHandle">The element handle representing the DOM element to interact with.</param>
        public PuppeteerElement(IElementHandle elementHandle)
        {
            _elementHandle = elementHandle;
        }

        /// <summary>
        /// Clicks the element asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ClickAsync()
        {
            await _elementHandle.ClickAsync();
        }

        /// <summary>
        /// Types the specified text into the element asynchronously.
        /// </summary>
        /// <param name="text">The text to type into the element.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task TypeAsync(string text)
        {
            await _elementHandle.TypeAsync(text);
        }

        /// <summary>
        /// Uploads a file to the element asynchronously (typically used for file input elements).
        /// </summary>
        /// <param name="filePath">The file path to upload.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UploadFileAsync(string filePath)
        {
            await _elementHandle.UploadFileAsync(filePath);
        }

        /// <summary>
        /// Finds a child element inside the current element by its CSS selector asynchronously.
        /// </summary>
        /// <param name="selector">The CSS selector of the element to find.</param>
        /// <returns>A task representing the asynchronous operation, containing the found element as a new PuppeteerElement, or null if not found.</returns>
        public async Task<IPageElement> FindElementAsync(string selector)
        {
            var elementHandle = await _elementHandle.QuerySelectorAsync(selector);

            if (elementHandle == null)
            {
                return null;
            }

            var element = new PuppeteerElement(elementHandle);

            return element;
        }

        public async Task<string> EvaluateFunctionAsync<T>(string script)
        {
            var text = await _elementHandle.EvaluateFunctionAsync<string>(script);
            return text;
        }
    }
}
