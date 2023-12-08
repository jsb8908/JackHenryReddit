namespace JackHenryReddit.Data.Models
{
    public class RedditAccessTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public long expires_in { get; set; }
        public string scope { get; set; }

    }
}
