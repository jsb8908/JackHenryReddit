using JackHenryReddit.Data.Models;

namespace JackHenryReddit.Data.Repositories
{
    public class RedditModelRepository : IRedditModelRepository
    {
        readonly IDataContext _dataContext;

        public RedditModelRepository(IDataContext dataContext) => _dataContext = dataContext;

        public async Task AddRedditResponseChildData<T>(IEnumerable<T> childData) where T : RedditResponseChildData
            => await Task.Run(() => _dataContext.Add(childData));

        public async Task<IEnumerable<T>> GetRedditResponseChildData<T>() where T : RedditResponseChildData
            => await Task.Run(() => _dataContext.Get<T>());

        public async Task ReplaceRedditResponseChildData<T>(IEnumerable<T> childData) where T : RedditResponseChildData
            => await Task.Run(() => _dataContext.Replace(childData));
    }
}
