namespace JackHenryReddit.Services.HttpClients
{
    public interface IRedditAPIHttpClient
    {
        Task<T?> ExecuteAsync<T>(string endPoint, HttpMethod method, HttpContent? data = null, int delayInSeconds = 0) where T : class, new();
    }
}
