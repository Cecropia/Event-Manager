using System.Collections.Generic;

namespace EventManager.BusinessLogic.Entities
{
    public class Subscriber
    {
        public string Name { get; set; }
        public SubscriberConfig Config { get; set; }
        public List<Subscription> Subscriptions { get; set; }
    }
}
