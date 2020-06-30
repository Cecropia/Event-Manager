using Newtonsoft.Json.Linq;
using System;

namespace EventManager.BusinessLogic.Entities
{
    public class Event
    {
        public string Name { get; set; }
        public string Payload { get; set; }
        public DateTime Timestamp { get; set; }
        public JObject ExtraParams { get; set; }

    }
}
