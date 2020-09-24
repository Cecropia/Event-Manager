using EventManager.BusinessLogic.Entities.Configuration;
using EventManager.BusinessLogic.Extensions;
using EventManager.BusinessLogic.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EventManager.BusinessLogic.Entities.Exceptions
{
    [System.Serializable]
    public class CustomAuthFailureException : System.Exception
    {
        public CustomAuthFailureException() { }
        public CustomAuthFailureException(string message) : base(message) { }
        public CustomAuthFailureException(string message, System.Exception inner) : base(message, inner) { }
        protected CustomAuthFailureException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
