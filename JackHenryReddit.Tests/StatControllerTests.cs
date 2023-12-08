using JackHenryReddit.Controllers;
using JackHenryReddit.Data.Models;
using JackHenryReddit.Services.DTOs;
using JackHenryReddit.Services.Services;
using Moq;
using Xunit.Sdk;
using static JackHenryReddit.Data.Models.RedditStatModel;

namespace JackHenryReddit.Tests
{
    [TestClass]
    public class StatControllerTests
    {
        List<RedditResponseChildDataDTO> _redditChildDataDTOs = new List<RedditResponseChildDataDTO>()
        {
            new RedditResponseChildDataDTO()
            {
                ups = 1,
                author = "author_1",
                created_utc = 1701982808
            },
            new RedditResponseChildDataDTO()
            {
                ups = 2,
                author = "author_1",
                created_utc = 1701982809
            },
            new RedditResponseChildDataDTO()
            {
                ups = 3,
                author = "author_3",
                created_utc = 1701982810
            },
            new RedditResponseChildDataDTO()
            {
                ups = 4,
                author = "author_4",
                created_utc = 1701982811
            },
            new RedditResponseChildDataDTO()
            {
                ups = 5,
                author = "author_5",
                created_utc = 1701982812
            },
        };

        Mock<IRedditService> _mockRedditService = new Mock<IRedditService>();

        [TestMethod]
        public async Task Test_GetPostsOrderedByUpVotes()
        {
            _mockRedditService.Setup(rs => rs.GetStats<RedditPostsWtihMostUpVotes>()).ReturnsAsync(_redditChildDataDTOs);

            var statController = new StatController(_mockRedditService.Object);

            var result = await statController.PostsOrderedByUpVotes(5);

            Assert.AreEqual(result.Count(), _redditChildDataDTOs.Count);

            Assert.IsTrue(result.First().name == _redditChildDataDTOs.MaxBy(r => r.ups).name);
        }

        [TestMethod]
        public async Task Test_UsersOrderedByTotalPosts()
        {
            _mockRedditService.Setup(rs => rs.GetStats<RedditUsersWithMostPosts>()).ReturnsAsync(_redditChildDataDTOs);

            var statController = new StatController(_mockRedditService.Object);

            var result = await statController.UsersOrderedByTotalPosts(5);

            Assert.AreEqual(result.Count(), _redditChildDataDTOs.Count);

            Assert.IsTrue(result.First().name == _redditChildDataDTOs.MaxBy(r => _redditChildDataDTOs.Count(rc => rc.name == r.name)).name);
        }

        [TestMethod]
        public async Task Test_TotalPosts()
        {
            _mockRedditService.Setup(rs => rs.GetStats<RedditPostsOrderedByDate>()).ReturnsAsync(_redditChildDataDTOs);

            var statController = new StatController(_mockRedditService.Object);

            var result = await statController.TotalPosts();

            Assert.AreEqual(result, _redditChildDataDTOs.Count);
        }

        [TestMethod]
        public async Task Test_EarliestPosts()
        {
            var orderedByDateDesc = _redditChildDataDTOs.OrderByDescending(r => r.CreatedUtc);

            _mockRedditService.Setup(rs => rs.GetStats<RedditPostsOrderedByDate>()).ReturnsAsync(orderedByDateDesc);

            var statController = new StatController(_mockRedditService.Object);

            int lastNum = 2;
            var result = await statController.EarliestPosts(lastNum);

            Assert.AreEqual(result.Count(), lastNum);
           
            Assert.IsTrue(result.First().created_utc == _redditChildDataDTOs.OrderBy(r => r.CreatedUtc).First().created_utc);
        }


        [TestMethod]
        public async Task Test_LatestPosts()
        {
            var orderedByDateDesc = _redditChildDataDTOs.OrderByDescending(r => r.CreatedUtc);
          
            _mockRedditService.Setup(rs => rs.GetStats<RedditPostsOrderedByDate>()).ReturnsAsync(orderedByDateDesc);

            var statController = new StatController(_mockRedditService.Object);

            int lastNum = 3;
            var result = await statController.LatestPosts(lastNum);

            Assert.AreEqual(result.Count(), lastNum);

            Assert.IsTrue(result.First().created_utc == _redditChildDataDTOs.OrderByDescending(r => r.CreatedUtc).First().created_utc);
        }
    }
}