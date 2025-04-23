using PuppeteerExtraSharp;
using PuppeteerSharp;
using System.Threading.Tasks;

namespace FBMarketBot.Facade
{
    public class BrowserLevel : IBrowserLevel
    {
        private Task<IPage> _page;

        public BrowserLevel()
        {
            _page = CreateNewPageAsync();
        }

        private async Task<IPage> CreateNewPageAsync()
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();

            var extra = new PuppeteerExtra();
            var page = await extra.LaunchAsync(new LaunchOptions());
            var newPage = await page.NewPageAsync();

            return newPage;
        }

        public async Task GoToAsync(string url)
        {
            var page = await _page;
            await page.GoToAsync(url);
        }

        public async Task Click(string selector)
        {
            var page = await _page;
            await page.ClickAsync(selector);
        }

        public async Task FindElement(string selector)
        {
            var page = await _page;
            await page.QuerySelectorAsync(selector);
        }

        public async Task GetScreenShot(string file)
        {
            var page = await _page;
            await page.ScreenshotAsync(file);
        }

        public async Task CloseAsync()
        {
            var page = await _page;
            await page.CloseAsync();
        }
    }
}
