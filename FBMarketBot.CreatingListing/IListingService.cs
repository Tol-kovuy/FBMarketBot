using FBMarketBot.GoLogin.ProfileDTOs;
using FBMarketBot.IoC;
using System;
using System.Threading.Tasks;

namespace FBMarketBot.CreatingListing
{
    /// <summary>
    /// Interface for the service responsible for creating listings.
    /// </summary>
    public interface IListingService : ITransientDependency
    {
        /// <summary>
        /// Asynchronously creates a new listing.
        /// </summary>
        Task CreateAsync(Profile profile);
    }
}