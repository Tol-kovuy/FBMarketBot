using FBMarketBot.IoC;
using System.Threading.Tasks;

namespace FBMarketBot.Publisher
{
    /// <summary>
    /// Defines a contract for publishing ads on Facebook with singleton lifetime dependency registration.
    /// </summary>
    public interface IFaceBookAdPublisher : ISingletonDependency
    {
        /// <summary>
        /// Asynchronously starts the process of publishing ads on Facebook.
        /// </summary>
        /// <returns>A task representing the asynchronous publishing operation.</returns>
        Task StartByScheduleAsync();
    }
}