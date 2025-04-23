using PuppeteerSharp;
using System.Threading.Tasks;

namespace FBMarketBot.PuppeteerExtraStealth
{
    public class BrowserInitializer : IBrowserInitializer
    {


        public async Task<IBrowser> GetBrowser()
        {
            await DownloadBrowserAsync();

        }

        private LaunchOptions GetLaunchOptions()
        {
            return new LaunchOptions
            {
                Headless = false,
            };
        }

        private async Task<IPage> GetPageOptions()
        {

        }

        private async Task DownloadBrowserAsync()
        {
            using (var browserFetcher = new BrowserFetcher())
            {
                await browserFetcher.DownloadAsync();
            }
        }
    }
}
