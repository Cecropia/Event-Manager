using System;

namespace EventManager.BusinessLogic.Entities.Exceptions
{
    [Serializable]
    public class CustomAuthFailureException : Exception
    {
        public CustomAuthFailureException() { }
        public CustomAuthFailureException(string message) : base(message) { }
        public CustomAuthFailureException(string message, Exception inner) : base(message, inner) { }
        protected CustomAuthFailureException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
