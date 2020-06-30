using System;
using System.Collections.Generic;
using System.Text;

namespace EventManager.BusinessLogic.Entities.Config
{
    public class EventSubscriberConfiguration
    {
        // note: must match the name in the `subscribers` object
        public string Name { get; set; } 
        public string Method { get; set; } = "POST";
        public string Endpoint { get; set; } = null;
    }
}
