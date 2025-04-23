using FBMarketBot.IoC;
using System.Threading.Tasks;

namespace FBMarketBot.Login
{
    public interface IFaceBookLogin : ISingletonDependency
    {
        Task LoginAsync();
    }
}
