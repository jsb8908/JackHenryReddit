
using Microsoft.Extensions.Logging;

namespace JackHenryReddit.Services.HttpClients
{
    public class RedditAuthHttpClient : RedditHttpClient, IRedditAuthHttpClient
    {
        public RedditAuthHttpClient(HttpClient httpClient, ILogger<RedditAuthHttpClient> logger)
            : base(httpClient, logger) { }
    }
}
