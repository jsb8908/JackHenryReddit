namespace JackHenryReddit.Services.HttpClients
{
    public interface IRedditAuthHttpClient
    {
        Task<T?> ExecuteAsync<T>(string endPoint, HttpMethod method, HttpContent? data = null, int delayInSeconds = 0) where T : class, new();
    }
}
