using System.Collections.Generic;

namespace EventManager.BusinessLogic.Entities.Configuration
{
    public class EventManagerConfiguration
    {
        public string EventReceptionPath { get; set; }
        public string ReplyEventPrefix { get; set; }
        public List<ExternalServiceConfiguration> ExternalServices { get; set; }
        public List<SubscriptionConfiguration> Subscriptions { get; set; }
    }
}
