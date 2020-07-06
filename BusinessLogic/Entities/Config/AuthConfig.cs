using System;
using System.Collections.Generic;
using System.Text;

namespace EventManager.BusinessLogic.Entities.Config
{
    public class AuthConfig
    {
        public string Type { get; set; }
        public string LoginEndpoint { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
