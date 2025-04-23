namespace FBMarketBot.Settings.Proxy
{
    /// <summary>
    /// Represents the settings required for configuring a proxy, implementing the IProxySettings interface.
    /// This interface defines the necessary properties for enabling and configuring a proxy server.
    /// </summary>
    public interface IProxySettings
    {
        bool Enabled { get; }
        string Host { get; }
        int Port { get; }
        string UserName { get; }
        string Password { get; }
    }
}
