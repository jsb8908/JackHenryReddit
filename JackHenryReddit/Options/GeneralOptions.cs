namespace JackHenryReddit.Options
{
    public class GeneralOptions
    {
        public const string General = "General";

        // Max frequency we will retriev subreddit data
        public int SubRedditRetrievalFrequencyInMilliSeconds { get; set; }
        // Frequency at which we calcualte statistics
        public int SubRedditStatFrequencyInMilliSeconds { get; set; }
    }
}
