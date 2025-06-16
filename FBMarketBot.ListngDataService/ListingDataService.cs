using FBMarketBot.GoLogin.ProfileDTOs;
using FBMarketBot.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FBMarketBot.ListngDataService
{
    public class ListingDataService : IListingDataService
    {
        private readonly ILogger _logger;
        private readonly Random _random = new Random();

        public ListingDataService(
            ILogger<IListingDataService> logger
            )
        {
            _logger = logger;
        }

        public IList<string> GetUnpostedListings(string profileId)
        {
            string allListingPath = Path.Combine(Directory.GetCurrentDirectory(), FilePaths.TitlePath);
            string postedFolderPath = Path.Combine(Directory.GetCurrentDirectory(), FilePaths.PostedListingPath);
            string profileFile = Path.Combine(postedFolderPath, $"{profileId}.txt");

            var allListings = File.Exists(allListingPath)
                ? File.ReadAllLines(allListingPath).ToList()
                : new List<string>();

            var postedListings = File.Exists(profileFile)
                ? File.ReadAllLines(profileFile).ToHashSet()
                : new HashSet<string>();

            var unpostedIndexes = allListings
                .Select((title, index) => new { title, index })
                .Where(x => !postedListings.Contains(x.title))
                .ToList();

            if (unpostedIndexes.Count > 0)
            {
                var shuffledListings = unpostedIndexes.OrderBy(x => _random.Next()).ToList();
                _logger.LogInformation($"Found {shuffledListings.Count} unposted listings for profile {profileId}");
                
                return shuffledListings
                    .Select(x => x.index.ToString())
                    .ToList();
            }

            File.WriteAllText(profileFile, string.Empty);
            _logger.LogInformation($"All listings have been posted for profile {profileId}. Resetting file and shuffling all listings.");

            var allIndexes = allListings
                .Select((title, index) => index.ToString())
                .OrderBy(rnd => _random.Next())
                .ToList();

            return allIndexes;
        }

        public void SavePostedListingByProfile(Profile profile)
        {
            string allListingPath = Path.Combine(Directory.GetCurrentDirectory(), FilePaths.TitlePath);

            CreateFolder();

            if (!int.TryParse(profile.NumberListingByProfile, out int index))
            {
                throw new ArgumentException("Invalid listing index format", nameof(profile.NumberListingByProfile));
            }

            if (!File.Exists(allListingPath))
            {
                throw new FileNotFoundException("All listing file not found.", allListingPath);
            }

            var allListings = File.ReadAllLines(allListingPath);
            if (index < 0 || index >= allListings.Length)
            {
                throw new IndexOutOfRangeException("Listing index is out of bounds.");
            }

            string listingName = allListings[index];

            string profileFilePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                Path.GetDirectoryName(FilePaths.PostedListingPath),
                $"{profile.Id}.txt"
            );

            using (StreamWriter writer = new StreamWriter(profileFilePath, append: true))
            {
                writer.WriteLine(listingName);
            }
        }

        private void CreateFolder()
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(),
                Path.GetDirectoryName(FilePaths.PostedListingPath));

            if (Directory.Exists(folderPath))
            {
                _logger.LogInformation($"Folder {folderPath} already exist.");
                return;
            }

            Directory.CreateDirectory(folderPath);
            _logger.LogInformation($"Folder {folderPath} already exist.");
        }

        private void CreateFile()
        {
            var filePath = FilePaths.PostedListingPath;

            if (File.Exists(filePath))
            {
                _logger.LogInformation($"File {filePath} already exist.");
                return;
            }

            File.Create(filePath).Close();
        }
    }
}
