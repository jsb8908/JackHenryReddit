# Project Title

Reddit Stats API

## Getting Started

Once built and running, the code will periodically log the Reddit API Rate Limit settings and any HTTP calls being made in the system.

There is a swagger endpoint (listed below) where various statistics and information about the data can be returned.

### Dependencies

.NET 7.0 SDK: https://dotnet.microsoft.com/download/dotnet/7.0
### Installing

```git clone https://github.com/jsb8908/JackHenryReddit.git ```

### Executing program

```cd JackHenryReddit\JackHenryReddit```

Edit appsettings.json: 

Insert BearerToken for Reddit API at RedditAPI.BearerToken

Or

Edit appsettings.json: 

Insert ClientId + ClientSecret at RedditAPI.ClientID + RedditAPI.ClientSecret

SubReddit: ```Comma separated list of subreddits```

SubRedditRetrievalFrequencyInMilliSeconds: ```Max frequency for checking the Reddit API, this is a ceiling for whatever rates the Reddit API return```

SubRedditStatFrequencyInMilliSeconds: ```How often we calculate statistics```

```dotent run```

### Testing program

To test, you can:

1. Point browser to url: http://localhost:5136/swagger

