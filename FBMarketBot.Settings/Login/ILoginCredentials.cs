namespace FBMarketBot.Settings.Login
{
    /// <summary>
    /// Defines a contract for accessing login credentials used for authentication.
    /// </summary>
    public interface ILoginCredentials
    {
        string Email { get; }
        string Password { get; }
    }
}