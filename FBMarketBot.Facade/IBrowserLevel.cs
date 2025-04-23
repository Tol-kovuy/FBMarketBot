using FBMarketBot.IoC;
using PuppeteerSharp;
using System.Threading.Tasks;

namespace FBMarketBot.Facade
{
    public interface IBrowserLevel : ISingletonDependency
    {
        Task<IPageLevel> GetBrowser();
    }
}
