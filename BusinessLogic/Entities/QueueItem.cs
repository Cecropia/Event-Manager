using System;
using System.Net;

namespace EventManager.Integration.Entities
{
    public class QueueItem
    {
        public Event Event { get; set; }
        public Subscription Subscription { get; set; }
        public HttpStatusCode Status { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime LastTry { get; set; }
    }
}
