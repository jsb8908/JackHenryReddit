using JackHenryReddit.Data.Models;

namespace JackHenryReddit.Data.Repositories
{
    public interface IRedditModelRepository
    {
        Task AddRedditResponseChildData<T>(IEnumerable<T> childData) where T : RedditResponseChildData;
        Task<IEnumerable<T>> GetRedditResponseChildData<T>() where T : RedditResponseChildData;
        Task ReplaceRedditResponseChildData<T>(IEnumerable<T> childData) where T : RedditResponseChildData;
    }
}