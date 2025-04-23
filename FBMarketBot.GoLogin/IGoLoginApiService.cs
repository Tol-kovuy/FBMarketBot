// Ignore Spelling: Api

using FBMarketBot.GoLogin.ProfileDTOs;
using FBMarketBot.IoC;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FBMarketBot.GoLogin
{
    public interface IGoLoginApiService : ITransientDependency
    {
        Task<string> StartProfileAsync(Profile profile);
        Task StopProfileAsync(Profile profile);
        Task<Profile> GetProfileByIdAsync(string currentProfileId);
        Task<IList<Profile>> GetAllProfilesAsync();
    }
}
