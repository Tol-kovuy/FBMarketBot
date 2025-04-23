namespace FBMarketBot.Settings.Proxy
{
    /// <summary>
    /// Represents the implementation of the IProxySettings interface.
    /// This class provides the necessary properties to configure and manage proxy settings.
    /// </summary>
    public class ProxySettings : IProxySettings
    {
        public bool Enabled { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
