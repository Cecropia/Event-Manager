using System;
using System.Collections.Generic;
using System.Text;

namespace EventManager.BusinessLogic.Entities.Config
{
    public class Config
    {
        public string BaseURL { get; set; } // mandatoy
        public string ClientKey { get; set; } // optional
        public string Secret { get; set; } // optional
        public int MaxRetries { get; set; } = 999;
        public int RequestRate { get; set; } = 312;
    }
}
