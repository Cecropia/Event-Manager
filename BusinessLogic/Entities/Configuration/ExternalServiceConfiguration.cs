using System;

namespace EventManager.BusinessLogic.Entities.Configuration
{
    public class ExternalServiceConfiguration
    {
        public string Name { get; set; }
        public Config Config { get; set; }
        public AuthConfig Auth { get; set; }
    }
}
