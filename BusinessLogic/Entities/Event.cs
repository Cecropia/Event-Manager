using Newtonsoft.Json.Linq;

namespace EventManager.Integration.Entities
{
    public class Event
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public JObject Structure { get; set; }
    }
}
