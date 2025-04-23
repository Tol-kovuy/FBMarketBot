// Ignore Spelling: Api

using FBMarketBot.GoLogin.ProfileDTOs;
using System.Diagnostics;
using System.IO;
using System.Net;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FBMarketBot.Settings.ApplicationSettingsAccessor;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace FBMarketBot.GoLogin
{
    public class GoLoginApiService : IGoLoginApiService
    {

        private readonly string _token;
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoLoginApiService> _logger;

        public GoLoginApiService(
            IApplicationSettingsAccessor settings,
            ILogger<GoLoginApiService> logger
            )
        {
            _token = settings.Get().GoLoginApiToken;
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public async Task<string> StartProfileAsync(Profile profile)
        {
            await EnsureGoLoginIsRunningAsync();

            var content = new StringContent("{\"profileId\": \"" + profile.Id + "\", \"sync\": true}", Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:36912/browser/start-profile");
            request.Headers.Add("Authorization", _token);
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            var json = JsonConvert.DeserializeObject<JObject>(responseContent);

            var property = json.GetValue("wsUrl");
            if (property == null)
            {
                _logger.LogInformation($"Error trying to start profile {profile.Name}.");
                return null;
            }

            _logger.LogInformation($"Profile {profile.Name} was started.");
            return property.Value<string>();
        }

        public async Task StopProfileAsync(Profile profile)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:36912/browser/stop-profile");
            var content = new StringContent("{\n    \"profileId\": \"" + profile.Id + "\"\n}", null, "application/json");
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogInformation($"Error trying to stop profile {profile.Name}.");
            }

            _logger.LogInformation($"Profile {profile.Name} was stopped.");
        }

        public async Task<IList<Profile>> GetAllProfilesAsync()
        {
            await EnsureGoLoginIsRunningAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.gologin.com/browser/v2");
            request.Headers.Add("Authorization", _token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<ProfilesResponse>(json, new JsonSerializerSettings
            {
            });

            if (result == null)
            {
                return new ProfilesResponse().Profiles;
            }

            return result.Profiles;
        }

        public async Task<Profile> GetProfileByIdAsync(string currentProfileId)
        {
            await EnsureGoLoginIsRunningAsync();

            var profiles = await GetAllProfilesAsync();

            var currentProfile = profiles.FirstOrDefault(profile => profile.Id == currentProfileId);

            _logger.LogInformation($"\n Profile: '\x1b[32m{currentProfile.Name}\x1b[0m'. \n profile Id: '{currentProfile.Id}'. \n With Proxy Name: '{currentProfile.Proxy.Username}' - '{currentProfile.Proxy.Host}:{currentProfile.Proxy.Port}'. \n");

            return currentProfile;
        }

        private async Task EnsureGoLoginIsRunningAsync()
        {
            await StartGoLoginProgramAsync();
        }

        private async Task StartGoLoginProgramAsync()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string exePath = Path.Combine(localAppData, "Programs", "GoLogin", "GoLogin.exe");
            string downloadUrl = "https://dl.gologin.com/gologin.exe";
            string tempInstallerPath = Path.Combine(Path.GetTempPath(), "gologin.exe");

            if (!File.Exists(exePath))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(downloadUrl, tempInstallerPath);
                    _logger.LogInformation("Downloaded: " + tempInstallerPath);
                }

                var installProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = tempInstallerPath,
                    UseShellExecute = true
                });

                installProcess.WaitForExit();
                await Task.Delay(10000);
            }

            bool alreadyRunning = Process.GetProcessesByName("GoLogin").Any();
            if (!alreadyRunning)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = exePath,
                        UseShellExecute = true
                    }
                };

                process.Start();
                await Task.Delay(10000);
            }
        }
    }
}
