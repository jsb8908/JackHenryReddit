using JackHenryReddit.Common;
using JackHenryReddit.Data.Models;
using JackHenryReddit.Data.Repositories;
using JackHenryReddit.Services.DTOs;
using JackHenryReddit.Services.HttpClients;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace JackHenryReddit.Services.Services
{
    public class RedditService : IRedditService
    {
        readonly IRedditAPIHttpClient _redditAPIHttpClient;
        readonly IRedditModelRepository _redditModelRepository;
        readonly ILogger<RedditService> _logger;
        static ConcurrentDictionary<string, PrevResponseHeaderValues> _prevResponseHeaders = new ConcurrentDictionary<string, PrevResponseHeaderValues>();

        class PrevResponseHeaderValues
        {
            string _lastAfter = string.Empty;
            int _lastCount = 0;

            public string LastAfter
            {
                get => _lastAfter.TrimAndCompare("") ? "" : $"&after={_lastAfter}";
                set => _lastAfter = value;
            }

            public string LastCount
            {
                get => $"&count={_lastCount}";
                set => _lastCount = int.Parse(value);
            }
            public int LastCountRaw => _lastCount;

            public string SubRedditURL(string url, string sort = "top", int limit = 10)
                => $"r/{url.Trim()}/{sort.Trim()}?limit={limit}" + LastAfter + LastCount;
        }

        // This service should be a Singleton, marking these static for emphasis
        static double RequestsUsed = 0;
        static double RequestsAvailable = 0;
        static double WillResetInSeconds = 0;

        // dont drop below this number of requests left, treat <= to this as no more requests left
        const double MinimumNumberOfRequestsAvailable = 2;
        // do not run faster than the Reddit API allows
        static int RateLimitDelayInSeconds => RequestsAvailable > MinimumNumberOfRequestsAvailable ? 0 : (int)WillResetInSeconds;

        public RedditService(ILogger<RedditService> logger,
                             IRedditAPIHttpClient redditAPIHttpClient,
                             IRedditModelRepository redditModelRepository)
        {
            _logger = logger;
            _redditAPIHttpClient = redditAPIHttpClient;
            _redditModelRepository = redditModelRepository;
        }

        private void SetRateLimits(RedditResponseModel? model)
        {
#pragma warning disable CS8629 // Nullable value type may be null.
            // our Extension method FirstValueToInt() makes this null agnostic
            double? used = model?._XRateLimitUsed.FirstValueToDouble();
            double? remaining = model?._XRateLimitRemaining.FirstValueToDouble();
            double? reset = model?._XRateLimitReset.FirstValueToDouble();
#pragma warning restore CS8629 // Nullable value type may be null.
            if (used > -1)
            {
                Interlocked.Exchange(ref RequestsUsed, used.Value);
            }
            if (remaining > -1)
            {
                Interlocked.Exchange(ref RequestsAvailable, remaining.Value);
            }
            if (reset > -1)
            {
                Interlocked.Exchange(ref WillResetInSeconds, reset.Value);
            }

            _logger.LogInfoJackHenryReddit($"Requests Used: {RequestsUsed}");
            _logger.LogInfoJackHenryReddit($"Requests Available: {RequestsAvailable}");
            _logger.LogInfoJackHenryReddit($"Will Reset In (s): {WillResetInSeconds}");
            _logger.LogInfoJackHenryReddit($"Rate Limit Delay (s): {RateLimitDelayInSeconds}");
        }

        public async Task<IEnumerable<RedditResponseChildDataDTO>> GetFromDatabase()
            => (await _redditModelRepository.GetRedditResponseChildData<RedditResponseChildData>())
               .AutoMap<RedditResponseChildDataDTO>();

        public async Task<RedditResponseDTO> GetFromServer(string subreddit, string sort, int limit)
        {
            PrevResponseHeaderValues prevHeaderValues = new PrevResponseHeaderValues();
            if (!_prevResponseHeaders.TryGetValue(subreddit, out prevHeaderValues))
            {
                prevHeaderValues = new PrevResponseHeaderValues();
            }

            var model = await _redditAPIHttpClient.ExecuteAsync<RedditResponseModel>(prevHeaderValues.SubRedditURL(subreddit, sort, limit), HttpMethod.Get, null, RateLimitDelayInSeconds);
            SetRateLimits(model);
            int totalSoFar = (prevHeaderValues.LastCountRaw + model.data.dist);
            prevHeaderValues.LastCount = totalSoFar.ToString();
            prevHeaderValues.LastAfter = model.data.after;
            _prevResponseHeaders.AddOrUpdate(subreddit, prevHeaderValues, (key, preValue) => prevHeaderValues);
            return model.AutoMap<RedditResponseDTO>();
        }

        public async Task SyncFromServerToDatabase(string subreddit, string sort, int limit)
        {
            var serverResponse = await GetFromServer(subreddit, sort, limit);
            var children = serverResponse.data.children.Select(c => c.data);
            await _redditModelRepository.AddRedditResponseChildData(children);
        }

        public async Task CalculateStats()
        {
            _logger.LogInfoJackHenryReddit("");

            var childData = await _redditModelRepository.GetRedditResponseChildData<RedditResponseChildData>();

            IEnumerable<RedditStatModel.RedditPostsWtihMostUpVotes> postsOrderedByVotesDesc = childData
                                                                                              .OrderByDescending(c => c.ups)
                                                                                              .AutoMap<RedditStatModel.RedditPostsWtihMostUpVotes>();

            IEnumerable<RedditStatModel.RedditUsersWithMostPosts> usersAndTheirPostsOrderedDesc = childData
                                                                                              .OrderByDescending(c => childData.Count(cd => cd.author == c.author))
                                                                                              .AutoMap<RedditStatModel.RedditUsersWithMostPosts>();

            IEnumerable<RedditStatModel.RedditPostsOrderedByDate> postsOrderedByDateDesc = childData
                                                                                              .OrderByDescending(c => DateTimeOffset.FromUnixTimeSeconds(c.created_utc).DateTime)
                                                                                              .AutoMap<RedditStatModel.RedditPostsOrderedByDate>();

            await _redditModelRepository.ReplaceRedditResponseChildData(postsOrderedByVotesDesc);

            await _redditModelRepository.ReplaceRedditResponseChildData(usersAndTheirPostsOrderedDesc);

            await _redditModelRepository.ReplaceRedditResponseChildData(postsOrderedByDateDesc);
        }

        public async Task<IEnumerable<RedditResponseChildDataDTO>> GetStats<T>() where T : RedditResponseChildData
            => (await _redditModelRepository.GetRedditResponseChildData<T>())
               .AutoMap<RedditResponseChildDataDTO>();
    }
}
