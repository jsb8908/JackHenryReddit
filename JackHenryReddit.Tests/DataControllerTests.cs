using JackHenryReddit.Controllers;
using JackHenryReddit.Services.DTOs;
using JackHenryReddit.Services.Services;
using Moq;
using Xunit.Sdk;

namespace JackHenryReddit.Tests
{
    [TestClass]
    public class DataControllerTests
    {
        static string _subreddit = "funny";
        List<RedditResponseChildDataDTO> _redditChildDataDTOs = new List<RedditResponseChildDataDTO>()
        {
            new RedditResponseChildDataDTO()
            {
                subreddit =_subreddit,
                ups = 1
            },
            new RedditResponseChildDataDTO()
            {
                subreddit = _subreddit,
                ups = 2
            },
            new RedditResponseChildDataDTO()
            {
                subreddit = _subreddit,
                ups = 3
            },
            new RedditResponseChildDataDTO()
            {
                subreddit = _subreddit,
                ups = 4
            },
            new RedditResponseChildDataDTO()
            {
                subreddit = _subreddit,
                ups = 5
            },
        };


        Mock<IRedditService> _mockRedditService = new Mock<IRedditService>();

        [TestMethod]
        public async Task Test_GetChildData()
        {
            _mockRedditService.Setup(rs => rs.GetFromDatabase()).ReturnsAsync(_redditChildDataDTOs);

            var dataController = new DataController(_mockRedditService.Object);

            var result = await dataController.GetChildData(_subreddit);

            Assert.AreEqual(result.Count(), _redditChildDataDTOs.Count);
        }
    }
}