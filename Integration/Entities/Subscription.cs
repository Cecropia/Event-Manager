namespace EventManager.Integration.Entities
{
    public class Subscription
    {
        public Subscriber Subscriber { get; set; }
        public Event EventName { get; set; }
        public string Method { get; set; }
        public string Endpoint { get; set; }

        // Que tipo de dato seria el Callback?
    }
}
