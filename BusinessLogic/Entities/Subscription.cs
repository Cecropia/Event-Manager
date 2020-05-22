using System;
using System.Collections.Generic;

namespace EventManager.Integration.Entities
{
    public class Subscription
    {
        public Subscriber Subscriber { get; set; }
        public Event Event { get; set; }
        public string Method { get; set; }
        public string Endpoint { get; set; }
        public List<Action> Callback { get; set; }
    }
}
