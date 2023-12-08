using JackHenryReddit.Data.Models;

namespace JackHenryReddit.Services.DTOs
{
    public class RedditResponseChildDataDTO : RedditResponseChildData
    {
        public DateTime CreatedUtc => DateTimeOffset.FromUnixTimeSeconds(created_utc).DateTime;
    }
}
