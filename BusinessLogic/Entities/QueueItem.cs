using System;
using System.Net;
using System.Threading;

namespace EventManager.BusinessLogic.Entities
{
    public class QueueItem
    {
        public Guid Guid { get; set; }
        public Event Event { get; set; }
        public Subscription Subscription { get; set; }
        public HttpStatusCode Status { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime LastTry { get; set; }
        public int Tries { get; set; }
    }
}
