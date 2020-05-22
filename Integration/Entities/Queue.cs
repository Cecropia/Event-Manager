using System;

namespace EventManager.Integration.Entities
{
    public class Queue
    {
        public Event EventName { get; set; }
        public Subscription Subscription { get; set; }
        public int Status { get; set; }
        // Status es un int?
        public DateTime Timestamp { get; set; }
        //El timestamp va a ser una fecha?
        //Que almacena lastry?
    }
}
