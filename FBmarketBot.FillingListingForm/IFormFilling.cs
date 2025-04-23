using FBMarketBot.GoLogin.ProfileDTOs;
using FBMarketBot.IoC;
using System;
using System.Threading.Tasks;

namespace FBMarketBot.FillingListingForm
{
    public interface IFormFilling : ISingletonDependency
    {
        Task PublishListingAsync(Profile profile);
    }
}
