using FBMarketBot.GoLogin.ProfileDTOs;
using FBMarketBot.IoC;
using System.Collections.Generic;

namespace FBMarketBot.ListngDataService
{
    public interface IListingDataService : ISingletonDependency
    {
        void SavePostedListingByProfile(Profile profile);
        IList<string> GetUnpostedListings(string profileId);
    }
}
