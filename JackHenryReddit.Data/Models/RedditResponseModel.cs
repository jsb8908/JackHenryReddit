using Newtonsoft.Json;
namespace JackHenryReddit.Data.Models
{
    public class RedditResponseModel
    {
        public string kind { get; set; } = string.Empty;
        public RedditResponseData data { get; set; } = new RedditResponseData();
        /// <summary>
        /// pproximate number of requests used in this period
        /// </summary>
        [JsonProperty("x-ratelimit-used")]
        public List<string> _XRateLimitUsed { get; set; } = new List<string>();
        /// <summary>
        /// Approximate number of requests left to use
        /// </summary>
        [JsonProperty("x-ratelimit-remaining")]
        public List<string> _XRateLimitRemaining { get; set; } = new List<string>();
        /// <summary>
        /// Approximate number of seconds to end of period
        /// </summary>
        [JsonProperty("x-ratelimit-reset")]
        public List<string> _XRateLimitReset { get; set; } = new List<string>();
    }

    public class RedditResponseData
    {
        public string after { get; set; } = string.Empty;
        public string before { get; set; } = string.Empty;
        public int dist { get; set; }
        public string modhash { get; set; } = string.Empty;
        public string geo_filter { get; set; } = string.Empty;
        public IEnumerable<RedditResponseChild> children { get; set; } = Enumerable.Empty<RedditResponseChild>();
    }

    public class RedditResponseChild
    {
        public string kind { get; set; } = string.Empty;
        public RedditResponseChildData data { get; set; } = new RedditResponseChildData();
    }

    public class RedditResponseChildData
    {
        public string id { get; set; }
        public string subreddit { get; set; } = string.Empty;
        public string author_fullname { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string author { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public double upvote_ratio { get; set; }
        public int num_comments { get; set; }
        public int ups { get; set; }
        public int downs { get; set; }
        public int subreddit_subscribers { get; set; }
        public long created_utc { get; set; } = DateTime.UtcNow.Ticks;
    }
}