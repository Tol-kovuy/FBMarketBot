using FBMarketBot.FillingListingForm;
using FBMarketBot.GoLogin.ProfileDTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FBMarketBot.CreatingListing
{
    /// <summary>
    /// Service for creating listings. 
    /// Uses a form-filling component to publish listings by schedule.
    /// </summary>
    public class ListingService : IListingService
    {
        private readonly IFormFilling _formFilling; 
        private readonly ILogger<ListingService> _logger;

        /// <summary>
        /// Initializes the ListingService with necessary dependencies.
        /// </summary>
        /// <param name="logger">Logger for logging service activities.</param>
        /// <param name="formFilling">The form-filling service used to submit listings.</param>
        public ListingService(
            ILogger<ListingService> logger,
            IFormFilling formFilling
            )
        {
            _formFilling = formFilling;
            _logger = logger;
        }

        /// <summary>
        /// Starts the process of creating a new listing.
        /// Logs a warning indicating the start time and triggers the form-filling operation.
        /// </summary>
        public async Task CreateAsync(Profile profile)
        {
            _logger.LogWarning($"\n\n\n\u001b[33mNew listing #{profile.NumberListingByProfile} start at {DateTime.Now} by {profile.Name}\u001b[0m\n\n\n");

            await _formFilling.PublishListingAsync(profile);
        }
    }
}
