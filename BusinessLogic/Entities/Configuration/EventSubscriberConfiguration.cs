namespace EventManager.BusinessLogic.Entities.Configuration
{
    public class EventSubscriberConfiguration
    {
        // note: must match the name in the `subscribers` object
        public string Name { get; set; } 
        public string Method { get; set; } = "POST";
        public string Endpoint { get; set; } = null;
        public bool Synchronous { get; set; } = false;
    }
}
