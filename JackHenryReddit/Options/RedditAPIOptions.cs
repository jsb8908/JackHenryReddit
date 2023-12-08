namespace JackHenryReddit.Options
{
    public class RedditAPIOptions
    {
        public const string RedditAPI = "RedditAPI";
        public string AuthUrl { get; set; }
        public string BaseUrl { get; set; }
        public string SubReddits { get; set; }
        public string Sort { get; set; }
        public int Limit { get; set; }
        // If BearerToken is set in appsettings, we will use that
        public string BearerToken { get; set; }
        // If no BearerToken set, we will grab the token and set it using ClientId + ClientSecret
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
