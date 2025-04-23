using FBMarketBot.GoLogin;
using FBMarketBot.GoLogin.ProfileDTOs;
using FBMarketBot.Settings.ApplicationSettingsAccessor;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FBMarketBot.BrowserAutomation
{
    public class PuppeteerBrowser : IBrowserAutomation
    {
        private IBrowser _browser;
        private IPage _page;
        private readonly IGoLoginApiService _goLoginApiService;
        private readonly ILogger<PuppeteerBrowser> _logger;

        public PuppeteerBrowser(
            IApplicationSettingsAccessor settings,
            ILogger<PuppeteerBrowser> logger,
            IGoLoginApiService goLoginApiService
            )
        {
            _logger = logger;
            _goLoginApiService = goLoginApiService;
        }

        public async Task InitializeAsync(Profile profile)
        {
            var wsUrl = await _goLoginApiService.StartProfileAsync(profile);

            var connectOptions = new ConnectOptions
            {
                BrowserWSEndpoint = wsUrl,
                IgnoreHTTPSErrors = true
            };

            _browser = await Puppeteer.ConnectAsync(connectOptions);
            _page = await _browser.NewPageAsync();

            await _page.SetViewportAsync(new ViewPortOptions
            {
                Width = 1920,
                Height = 1080
            });
        }

        /// <summary>
        /// Navigates to the specified URL in the browser. Ensures that the browser and page are initialized before navigating.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        public async Task GoToAsync(string url)
        {
            await _page.GoToAsync(url);
        }

        /// <summary>
        /// Finds a single element on the page by its CSS selector. Ensures that the browser and page are initialized before searching.
        /// </summary>
        /// <param name="selector">The CSS selector of the element to find.</param>
        /// <returns>A task representing the asynchronous operation, containing the found element as an IPageElement.</returns>
        public async Task<IPageElement> FindElementAsync(string selector)
        {
            var elementHandle = await _page.QuerySelectorAsync(selector);

            if (elementHandle == null)
            {
                _logger.LogError($"Element by selector '{selector}' not found.");
                return null;
            }

            var element = new PuppeteerElement(elementHandle);

            return element;
        }

        /// <summary>
        /// Finds all elements on the page matching the given CSS selector. Ensures that the browser and page are initialized before searching.
        /// </summary>
        /// <param name="selector">The CSS selector of the elements to find.</param>
        /// <returns>A task representing the asynchronous operation, containing an array of IPageElement objects for the matching elements.</returns>
        public async Task<IPageElement[]> FindAllElementsAsync(string selector)
        {
            var elements = await _page.QuerySelectorAllAsync(selector);
            if (elements == null)
            {
                _logger.LogError($"Elements not found for Selector: {selector}");
                return null;
            }

            return elements.Select(e => new PuppeteerElement(e)).ToArray();
        }

        /// <summary>
        /// Finds all elements on the page that match the provided XPath expression. Ensures that the browser and page are initialized before searching.
        /// </summary>
        /// <param name="selector">The XPath selector to find elements by.</param>
        /// <returns>A task representing the asynchronous operation, containing an array of IPageElement objects for the matching elements.</returns>
        public async Task<IPageElement[]> FindAllXPathAsync(string selector)
        {
            var elements = await _page.XPathAsync(selector);
            if (elements == null)
            {
                _logger.LogError($"Element not found for XPath: {selector}");
                return null;
            }

            return elements.Select(e => new PuppeteerElement(e)).ToArray();
        }

        /// <summary>
        /// Retrieves all cookies associated with the current page. Ensures that the browser and page are initialized before retrieving the cookies.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a collection of cookies.</returns>
        public async Task<IEnumerable<Cookie>> GetCookiesAsync()
        {
            var cookies = await _page.GetCookiesAsync();
            var netCookies = cookies.Select(c => new Cookie(c.Name, c.Value, c.Path, c.Domain));

            return netCookies;
        }

        /// <summary>
        /// Retrieves the current URL of the page. Ensures that the browser and page are initialized before retrieving the URL.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the current URL as a string.</returns>
        public async Task<string> GetCurrentUrl()
        {
            return _page.Url;
        }

        /// <summary>
        /// Waits for an element to appear on the page that matches the provided XPath expression. Ensures that the browser and page are initialized before waiting.
        /// </summary>
        /// <param name="selector">The XPath selector to wait for.</param>
        /// <returns>A task representing the asynchronous operation, containing the found element as an IPageElement.</returns>
        public async Task<IPageElement> WaitForXPathAsync(string selector)
        {
            var elementHandle = await _page.WaitForXPathAsync(selector);
            if (elementHandle == null)
            {
                _logger.LogError($"Element not found for XPath: {selector}");
                return null;
            }

            var element = new PuppeteerElement(elementHandle);

            return element;
        }

        public async Task<IPageElement> WaitForSelectorAsync(string selector)
        {
            var elementHandle = await _page.WaitForSelectorAsync(selector);
            var element = new PuppeteerElement(elementHandle);
            if (elementHandle == null)
            {
                _logger.LogError($"Element not found for Selector: {selector}");
                return null;
            }

            return element;
        }
    }
}
