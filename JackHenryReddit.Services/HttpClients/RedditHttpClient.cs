using JackHenryReddit.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JackHenryReddit.Services.HttpClients
{
    public abstract class RedditHttpClient
    {
        protected readonly HttpClient _httpClient;
        protected readonly ILogger<RedditHttpClient> _logger;

        public RedditHttpClient(HttpClient httpClient,
                                ILogger<RedditHttpClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public virtual async Task<T?> ExecuteAsync<T>(string endPoint, HttpMethod method, HttpContent? data = null, int delayInSeconds = 0) where T : class, new()
        {
            //rate limit our requests if needed
            await Task.Delay(delayInSeconds);

            HttpResponseMessage response = method.Method switch
            {
                "GET" => await _httpClient.GetAsync(endPoint),
                "POST" => await _httpClient.PostAsync(endPoint, data),
                // fill in rest of verbs as necessary
                _ => throw new NotImplementedException()
            };

            string responseContentAsString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Response Code:{response.StatusCode}. Response Content:{responseContentAsString}");
            }
            var headersAsJsonString = JsonConvert.SerializeObject(response.Headers.ToDictionary(k => k.Key, v => v.Value));
            var responseAsJObject = JObject.Parse(responseContentAsString);
            var headerValuesAsJObject = JObject.Parse(headersAsJsonString);

            responseAsJObject.Merge(headerValuesAsJObject, new JsonMergeSettings()
            {
                PropertyNameComparison = StringComparison.OrdinalIgnoreCase,
                MergeArrayHandling = MergeArrayHandling.Union
            });

            string asJson = JsonConvert.SerializeObject(responseAsJObject);
            return asJson.FromJson<T>();
        }
    }
}
