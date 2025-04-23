using FBMarketBot.IoC;
using PuppeteerSharp;
using System.Threading.Tasks;

namespace FBMarketBot.PuppeteerExtraStealth
{
    public interface IBrowserInitializer : ISingletonDependency
    {
        Task<IBrowser> GetBrowser();
    }
}
