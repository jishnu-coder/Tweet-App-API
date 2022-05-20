namespace Tweet_App_API.Model
{
    public class TweetAppDBSettings : ITweetAppDBSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string UserCollectionName { get; set; } = null!;

        public string TweetCollectionName { get; set; } = null!;
    }

    public interface ITweetAppDBSettings
    {
        public string UserCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string TweetCollectionName { get; set; }
    }

}
