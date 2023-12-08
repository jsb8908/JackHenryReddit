using JackHenryReddit.Common;
using JackHenryReddit.Data.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JackHenryReddit.Services.HttpClients
{
    public class RedditAPIHttpClient : RedditHttpClient, IRedditAPIHttpClient
    {
        private readonly IRedditAuthHttpClient _redditAuthClient;

        public RedditAPIHttpClient(HttpClient httpClient, ILogger<RedditAPIHttpClient> logger, IRedditAuthHttpClient redditAuthHttpClient)
            : base(httpClient, logger) => _redditAuthClient = redditAuthHttpClient;

        public override async Task<T?> ExecuteAsync<T>(string endPoint, HttpMethod method, HttpContent? data = null, int delayInSeconds = 0) where T : class
        {
            try
            {
                if (_httpClient.DefaultRequestHeaders.Authorization == null || _httpClient.DefaultRequestHeaders.Authorization.Parameter.TrimAndCompare(""))
                {
                    await AcquireAndSetAuthToken();
                }
                return await base.ExecuteAsync<T>(endPoint, method, data, delayInSeconds);
            }
            catch (Exception ex) when (ex.Message.Contains($"{HttpStatusCode.Unauthorized}"))
            {
                await AcquireAndSetAuthToken();
                return await base.ExecuteAsync<T>(endPoint, method, data, delayInSeconds);
            }
        }

        async Task AcquireAndSetAuthToken()
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                                                    {
                                                        {"grant_type", "client_credentials" }
                                                    });

            var tokenResponse = await _redditAuthClient.ExecuteAsync<RedditAccessTokenResponse>("access_token", HttpMethod.Post, content);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse?.access_token);
        }
    }
}
