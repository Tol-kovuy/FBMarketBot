namespace FBMarketBot.Settings.Login
{
    /// <summary>
    /// Represents login credentials for authentication, implementing the ILoginCredentials interface.
    /// </summary>
    public class LoginCredentials : ILoginCredentials
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}