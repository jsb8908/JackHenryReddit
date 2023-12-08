using JackHenryReddit.Data.Models;
using JackHenryReddit.Services.DTOs;
using JackHenryReddit.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace JackHenryReddit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatController : ControllerBase
    {
        private readonly IRedditService _redditService;
        public StatController(IRedditService redditService) => _redditService = redditService;

        [Route("PostsOrderedByUpVotes/{maxResults:int?}")]
        [HttpGet]
        public async Task<IEnumerable<RedditResponseChildDataDTO?>> PostsOrderedByUpVotes(int maxResults = 10)
            // .Take() is not ideal, better to push the select/criteria into the Repo or Service layer when proper data store is implemented
            => (await _redditService.GetStats<RedditStatModel.RedditPostsWtihMostUpVotes>()).Take(maxResults);

        [Route("UsersOrderedByTotalPosts/{maxResults:int?}")]
        [HttpGet]
        public async Task<IEnumerable<RedditResponseChildDataDTO>> UsersOrderedByTotalPosts(int maxResults = 10)
            => (await _redditService.GetStats<RedditStatModel.RedditUsersWithMostPosts>()).Take(maxResults);

        [Route("TotalPosts")]
        [HttpGet]
        public async Task<int> TotalPosts()
            => (await _redditService.GetStats<RedditStatModel.RedditPostsOrderedByDate>()).Count();

        [Route("EarliestPosts/{lastNum:int?}")]
        [HttpGet]
        public async Task<IEnumerable<RedditResponseChildDataDTO>> EarliestPosts(int lastNum = 1)
           => (await _redditService.GetStats<RedditStatModel.RedditPostsOrderedByDate>()).Reverse().Take(lastNum);

        [Route("LatestPosts/{lastNum:int?}")]
        [HttpGet]
        public async Task<IEnumerable<RedditResponseChildDataDTO>> LatestPosts(int lastNum = 1)
           => (await _redditService.GetStats<RedditStatModel.RedditPostsOrderedByDate>()).Take(lastNum);
    }
}
