using System;
using System.Collections.Generic;
using System.Text;

namespace EventManager.BusinessLogic.Entities.Config
{
    public class SubscriptionConfiguration
    {
        public string EventName { get; set; }
        public List<EventSubscriberConfiguration> Subscribers { get; set; }
    }
}
