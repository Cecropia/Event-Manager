namespace EventManager.Integration.Entities
{
    public class SubscriberConfig
    {
        public string ClientKey { get; set; }
        public string Secret { get; set; }
        public int MaxTries { get; set; }
        //Tipo de dato del request rate?
    }
}
