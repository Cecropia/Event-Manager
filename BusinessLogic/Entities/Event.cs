using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace EventManager.BusinessLogic.Entities
{
    public class Event
    {
        public string Name { get; set; }
        public string Payload { get; set; }
        public DateTime Timestamp { get; set; }
        public JObject ExtraParams { get; set; }
        public Dictionary<string, string> UrlTemplateValues { get; set; }
        public Dictionary<string, IEnumerable<string>> ContextHeaders { get; set; }
    }
}
