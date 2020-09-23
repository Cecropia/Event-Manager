using System;
using System.Collections.Generic;
using System.Net.Http;

namespace EventManager.BusinessLogic.Entities
{
    public class CustomAuthProviderRegistry
    {
        private Dictionary<string, Action<HttpRequestMessage, Subscription>> authRegister;

        public CustomAuthProviderRegistry()
        {
            this.authRegister = new Dictionary<string, Action<HttpRequestMessage, Subscription>>();
        }

        public void AddAuthProvider(string name, Action<HttpRequestMessage, Subscription> act)
        {
            this.authRegister.Add(name, act);
        }

        public Action<HttpRequestMessage, Subscription> GetAuthProvider(string name)
        {
            this.authRegister.TryGetValue(name, out Action<HttpRequestMessage, Subscription> value);
            return value;
        }


    }
}
