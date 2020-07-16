using System.Collections.Generic;

namespace EventManager.BusinessLogic.Entities.Configuration
{
    public class EventManagerConfiguration
    {
        public List<SubscriberConfiguration> Subscribers { get; set; }
        public List<SubscriptionConfiguration> Subscriptions { get; set; }
    }
}
