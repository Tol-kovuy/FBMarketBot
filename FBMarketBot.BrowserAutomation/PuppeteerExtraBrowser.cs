using FBMarketBot.GoLogin;
using FBMarketBot.Settings.ApplicationSettingsAccessor;
using FBMarketBot.Settings.Proxy;
using FBMarketBot.Settings.Schedule;
using FBMarketBot.Shared;
using Microsoft.Extensions.Logging;
using PuppeteerExtraSharp;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FBMarketBot.BrowserAutomation
{
    public class PuppeteerExtraBrowser //: IBrowserAutomation
    {
        private PuppeteerExtra _puppeteerExtra;
        private IBrowser _browser;
        private IPage _page;
        private readonly IProxySettings _proxySettings;
        private readonly IGoLoginApiService _goLoginApiService;
        private readonly IWeeklyPostSchedule _weeklyPostSchedule;
        private readonly ILogger<PuppeteerExtraBrowser> _logger;

        public PuppeteerExtraBrowser(
            IApplicationSettingsAccessor settings,
            ILogger<PuppeteerExtraBrowser> logger,
            IGoLoginApiService goLoginApiService
            )
        {
            _proxySettings = settings.Get().ProxySettings;
            _weeklyPostSchedule = settings.Get().WeeklyPostSchedule;
            _logger = logger;
            _puppeteerExtra = new PuppeteerExtra();
            _goLoginApiService = goLoginApiService;
        }

        public async Task InitializeAsync()
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

            // Adding the Stealth plugin for bypassing anti-bots
            // This will activate all these plugins and they will automatically start applying browser 
            // - level cloaking to bypass various detectors such as WebDriver, User - Agent, WebGL and others.
            // Important Note: These plugins are specifically designed to hide signs of automation, such as opening the browser 
            // via Puppeteer or the presence of WebDriver.
            //_puppeteerExtra = _puppeteerExtra.Use(new StealthPlugin());

            var args = GetLaunchArguments();

            try
            {
                _browser = await _puppeteerExtra.LaunchAsync(new LaunchOptions
                {
                    Headless = false,
                    UserDataDir = FilePaths.RelativeUserBrowserProfilePath,
                    Args = args
                });

                _page = await _browser.NewPageAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error launching browser: {ex.Message}");
                _logger.LogError(ex.StackTrace);
            }

            _page = await _browser.NewPageAsync();

            // Size of browsers screen
            await _page.SetViewportAsync(new ViewPortOptions
            {
                Width = 1920,
                Height = 1080
            });

            await _page.SetUserAgentAsync(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36"
            );

            // Set additional headers to mask behavior
            await _page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
            {
                { "Accept-Language", "en-US,en;q=0.9" },
                { "Referer", "https://www.google.com/" },
                { "Connection", "keep-alive" },
                { "Cache-Control", "no-cache"  }
            });

            // Authenticate proxy if required
            if (_proxySettings.Enabled)
            {
                if (string.IsNullOrEmpty(_proxySettings.UserName))
                {
                    throw new Exception("Proxy UserName empty.");
                }

                await _page.AuthenticateAsync(new Credentials
                {
                    Username = _proxySettings.UserName,
                    Password = _proxySettings.Password
                });
                _logger.LogInformation($"Proxy authenticate by user name {_proxySettings.UserName}");
            }
        }

        private string[] GetLaunchArguments()
        {
            var args = new List<string>
                {
                    "--disable-notifications",               // Disable browser notifications
                    "--disable-infobars",                    // Remove "Chrome is being controlled" message
                    //"--disable-web-security",                // Disable web security
                    "--start-maximized",                     // Open browser in maximized mode
                    "--log-level=3",                         // Reduce console logs
                    "--use-fake-ui-for-media-stream",        // Auto-grant permissions for media (camera, microphone)
                    "--use-fake-device-for-media-stream",    // Use a fake media device (for camera and microphone)
                    "--no-sandbox",                          // Disable sandboxing (often needed in CI environments)
                    "--disable-features=SitePerProcess",     // Disable security features that may block the file input
                    "--disable-setuid-sandbox",              // Disable setuid sandbox
                    //"--disable-features=IsolateOrigins,site-per-process",  // Disable isolation
                    "--disable-dev-shm-usage",               // Disables the use of /dev/shm (shared memory) in Linux environments
                    "--disable-extensions",                   // Disables browsers extensions
                    "--disable-features=BlockThirdPartyCookies"
                };

            if (_proxySettings.Enabled)
            {
                args.Add($"--proxy-server={_proxySettings.Host}:{_proxySettings.Port}");
                _logger.LogInformation($"Proxy is enabled with Host - {_proxySettings.Host} " +
                    $"and Port - {_proxySettings.Port}");
            }

            return args.ToArray();
        }

        /// <summary>
        /// Ensures that the browser and page are initialized. If they are not initialized, it calls InitializeAsync to initialize them.
        /// </summary>
        private async Task EnsureInitializedAsync()
        {
            if (_browser == null || _page == null)
            {
                await InitializeAsync();
            }
        }

        /// <summary>
        /// Navigates to the specified URL in the browser. Ensures that the browser and page are initialized before navigating.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        public async Task GoToAsync(string url)
        {
            await EnsureInitializedAsync();
            await _page.GoToAsync(url);
        }

        /// <summary>
        /// Finds a single element on the page by its CSS selector. Ensures that the browser and page are initialized before searching.
        /// </summary>
        /// <param name="selector">The CSS selector of the element to find.</param>
        /// <returns>A task representing the asynchronous operation, containing the found element as an IPageElement.</returns>
        public async Task<IPageElement> FindElementAsync(string selector)
        {
            await EnsureInitializedAsync();
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
            await EnsureInitializedAsync();
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
            await EnsureInitializedAsync();
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
            await EnsureInitializedAsync();
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
            await EnsureInitializedAsync();
            return _page.Url;
        }

        /// <summary>
        /// Waits for an element to appear on the page that matches the provided XPath expression. Ensures that the browser and page are initialized before waiting.
        /// </summary>
        /// <param name="selector">The XPath selector to wait for.</param>
        /// <returns>A task representing the asynchronous operation, containing the found element as an IPageElement.</returns>
        public async Task<IPageElement> WaitForXPathAsync(string selector)
        {
            await EnsureInitializedAsync();
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
