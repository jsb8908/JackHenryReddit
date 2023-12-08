using JackHenryReddit.Common;
using JackHenryReddit.Options;
using JackHenryReddit.Services.HttpClients;
using JackHenryReddit.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static IdentityModel.ClaimComparer;

namespace JackHenryReddit.HostedServices
{
    public class SubRedditRetrieval : BackgroundService
    {
        readonly ILogger<SubRedditRetrieval> _logger;
        readonly IServiceProvider _serviceProvider;
        readonly RedditAPIOptions _redditAPIOptions;

        public SubRedditRetrieval(IServiceProvider serviceProvider,
                                  IOptions<RedditAPIOptions> redditAPIOptions,
                                  ILogger<SubRedditRetrieval> logger)
        {
            _serviceProvider = serviceProvider;
            _redditAPIOptions = redditAPIOptions.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInfoJackHenryReddit("");
            using (var scope = _serviceProvider.CreateScope())
            {
                var redditService = _serviceProvider.GetRequiredService<IRedditService>();
                var redditAPIOptions = _serviceProvider.GetRequiredService<IOptions<RedditAPIOptions>>().Value;
                var generalOptions = _serviceProvider.GetRequiredService<IOptions<GeneralOptions>>().Value;

                //when this delay is set to 0, the service will run as fast as the Reddit API rate limit allows
                int delayInMilliSeconds = generalOptions.SubRedditRetrievalFrequencyInMilliSeconds;
                List<string> subreddits = redditAPIOptions.SubReddits.FromSeparatedString();

                while (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInfoJackHenryReddit("SyncFromServerToDatabase");

                    // sync each subreddit in parallel
                    await Parallel.ForEachAsync(subreddits,cancellationToken, async (subreddit, ct) =>
                    {
                        await redditService.SyncFromServerToDatabase(subreddit, _redditAPIOptions.Sort, _redditAPIOptions.Limit);
                    });

                    await Task.Delay(delayInMilliSeconds, cancellationToken);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInfoJackHenryReddit("");
            await base.StopAsync(cancellationToken);
        }
    }
}
