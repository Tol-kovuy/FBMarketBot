namespace FBMarketBot.Settings.Selectors.LoginSlctrs
{
    /// <summary>
    /// Represents the interface for defining selectors used in the login process.
    /// This interface provides the necessary selectors for interacting with login form elements.
    /// </summary>
    public class LoginSelectors : ILoginSelectors
    {
        public string EmailById { get; set; }
        public string PasswordById { get; set; }
        public string ClickLoginByName { get; set; }
    }
}
