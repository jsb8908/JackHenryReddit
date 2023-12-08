using JackHenryReddit.Data.Models;
using JackHenryReddit.Services.DTOs;

namespace JackHenryReddit.Services.Services
{
    public interface IRedditService
    {
        Task<RedditResponseDTO> GetFromServer(string subreddit, string sort, int limit);
        Task SyncFromServerToDatabase(string subreddit, string sort, int limit);
        Task<IEnumerable<RedditResponseChildDataDTO>> GetFromDatabase();
        Task CalculateStats();
        Task<IEnumerable<RedditResponseChildDataDTO>> GetStats<T>() where T : RedditResponseChildData;
    }
}
