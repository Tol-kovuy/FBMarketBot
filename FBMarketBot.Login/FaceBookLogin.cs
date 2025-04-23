using FBMarketBot.BrowserAutomation;
using FBMarketBot.Settings;
using FBMarketBot.Settings.ApplicationSettingsAccessor;
using FBMarketBot.Settings.AppSettings;
using FBMarketBot.Settings.Selectors;
using FBMarketBot.Settings.Selectors.AccessorSlctrs;
using FBMarketBot.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FBMarketBot.Login
{
    /// <summary>
    /// Implements functionality to log in to Facebook using browser automation.
    /// </summary>
    public class FaceBookLogin : IFaceBookLogin
    {
        private readonly IApplicationSettingsAccessor _settings;
        private readonly ISelectorsAccessors _selectors;
        private readonly ILogger<FaceBookLogin> _logger;
        private readonly IBrowserAutomation _browser;

        /// <summary>
        /// Initializes a new instance of the FaceBookLogin class with required dependencies.
        /// </summary>
        /// <param name="settings">Accessor for application settings, including login credentials.</param>
        /// <param name="selectors">Accessor for selectors used in the login process.</param>
        /// <param name="logger">Logger instance for logging operations and errors.</param>
        /// <param name="browser">Browser automation service for interacting with the webpage.</param>
        public FaceBookLogin(
            IApplicationSettingsAccessor settings,
            ISelectorsAccessors selectors,
            ILogger<FaceBookLogin> logger,
            IBrowserAutomation browser
            )
        {
            _settings = settings;
            _selectors = selectors;
            _logger = logger;
            _browser = browser;
        }

        /// <summary>
        /// Asynchronously logs in to Facebook using provided credentials and browser automation.
        /// </summary>
        /// <returns>A task representing the asynchronous login operation.</returns>
        public async Task LoginAsync()
        {
            try
            {
                await _browser.GoToAsync(FilePaths.MainPageUrl);

                var selectors = _selectors.Get();

                if (await IsLogIn())
                {
                    _logger.LogInformation("Successfully logged in to FaceBook.");
                    return;
                }

                var loginCredentials = _settings.Get();

                await GetLoginForm(selectors, loginCredentials);

                await _settings.DelayBetweenActionsAsync();

                _logger.LogInformation("Confirm login in your device or FaceBook account and press Enter to continue...");
                WaitForUserConfirmation();

                if (await IsLogIn())
                {
                    _logger.LogInformation("Successfully logged in to FaceBook.");
                }
                else
                {
                    _logger.LogError("Failed to log in to Facebook: Check Facebook credentials and restart application.");
                    await LoginAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Asynchronously fills and submits the Facebook login form with provided credentials.
        /// </summary>
        /// <param name="selectors">Selectors used to locate login form elements.</param>
        /// <param name="loginCredentials">Application settings containing email and password.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task GetLoginForm(ISelectors selectors, IApplicationSettings loginCredentials)
        {
            var emailField = await _browser.FindElementAsync(selectors.LoginSelectors.EmailById);
            await emailField.ClickAsync();
            await emailField.TypeTextLikeHumanAsync(loginCredentials.LoginCredentials.Email);

            var passwordField = await _browser.FindElementAsync(selectors.LoginSelectors.PasswordById);
            await passwordField.ClickAsync();
            await passwordField.TypeTextLikeHumanAsync(loginCredentials.LoginCredentials.Password);

            var loginButton = await _browser.FindElementAsync(selectors.LoginSelectors.ClickLoginByName);
            _logger.LogInformation($"Attempting to log in to FaceBook with email '{loginCredentials.LoginCredentials.Email}'");
            await loginButton.ClickAsync();
        }

        /// <summary>
        /// Waits for user confirmation by pressing the Enter key.
        /// </summary>
        private void WaitForUserConfirmation()
        {
            ConsoleKey key;
            do
            {
                key = Console.ReadKey(true).Key;
            } while (key != ConsoleKey.Enter);
        }

        /// <summary>
        /// Asynchronously checks if the user is logged in by inspecting cookies.
        /// </summary>
        /// <returns>A task that resolves to true if logged in, false otherwise.</returns>
        private async Task<bool> IsLogIn()
        {
            var cookies = await _browser.GetCookiesAsync();
            var isLogin = cookies.Any(c => c.Name.Contains("c_user"));

            return isLogin;
        }
    }
}