namespace FBMarketBot.GoLogin.ProfileDTOs
{
    public class Profile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string UserAgent { get; set; }
        public string Resolution { get; set; }
        public string Language { get; set; }
        public string Os { get; set; }
        public string OsVersion { get; set; }
        public Proxy Proxy { get; set; }
        public string NumberListingByProfile { get; set; }
    }
}
