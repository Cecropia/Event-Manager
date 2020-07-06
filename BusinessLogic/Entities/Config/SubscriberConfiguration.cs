namespace EventManager.BusinessLogic.Entities.Config
{
    public class SubscriberConfiguration
    {
        public string Name { get; set; }
        public Config Config { get; set; }
        public AuthConfig Auth { get; set; }
    }
}
