namespace EventManager.Integration.Entities
{
    public class SubscriberConfig
    {
        public string ClientKey { get; set; }
        public string Secret { get; set; }
        public int MaxTries { get; set; }
        public int RequestRate { get; set; }
    }
}
