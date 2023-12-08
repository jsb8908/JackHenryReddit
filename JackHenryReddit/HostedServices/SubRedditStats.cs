using JackHenryReddit.Common;
using JackHenryReddit.Options;
using JackHenryReddit.Services.HttpClients;
using JackHenryReddit.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JackHenryReddit.HostedServices
{
    public class SubRedditStats: BackgroundService
    {
        readonly ILogger<SubRedditStats> _logger;
        readonly IServiceProvider _serviceProvider;

        public SubRedditStats(IServiceProvider serviceProvider,
                                  ILogger<SubRedditStats> logger)
        {
            _serviceProvider = serviceProvider;
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

                 int delayInMilliSeconds = generalOptions.SubRedditStatFrequencyInMilliSeconds;

                while (!cancellationToken.IsCancellationRequested)
                {
                    await redditService.CalculateStats();

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
