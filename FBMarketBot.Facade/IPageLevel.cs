using FBMarketBot.IoC;
using PuppeteerSharp;
using System.Threading.Tasks;

namespace FBMarketBot.Facade
{
    public interface IPageLevel : ISingletonDependency
    {
        Task GoToAsync(string url);
        Task ClickAsync(string selector);
        Task FindElementAsync(string selector);
        Task GetScreenShot(string file);
        Task CloseAsync();
    }
}
