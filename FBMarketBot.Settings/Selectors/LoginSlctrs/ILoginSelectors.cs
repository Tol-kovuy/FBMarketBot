namespace FBMarketBot.Settings.Selectors.LoginSlctrs
{
    /// <summary>
    /// Represents the interface for defining selectors used in the login process.
    /// This interface provides the necessary selectors for interacting with login form elements.
    /// </summary>
    public interface ILoginSelectors
    {
        string EmailById { get; }
        string PasswordById { get; }
        string ClickLoginByName { get; }
    }
}
