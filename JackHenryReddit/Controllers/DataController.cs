using JackHenryReddit.Common;
using JackHenryReddit.Services.DTOs;
using JackHenryReddit.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JackHenryReddit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IRedditService _redditService;
        public DataController(IRedditService redditService) => _redditService = redditService;

        [Route("GetChildData/{subreddit}/{maxResults:int?}")]
        [HttpGet]
        public async Task<IEnumerable<RedditResponseChildDataDTO>> GetChildData(string subreddit, int maxResults = 10)
            => (await _redditService.GetFromDatabase()).Where(r => r.subreddit.TrimAndCompare(subreddit)).Take(10);
    }
}
