using System;
using System.Collections.Generic;
using System.Net.Http;

namespace EventManager.BusinessLogic.Entities
{
    public class CustomAuthProviderRegistry
    {
        private Dictionary<string, Action<HttpRequestMessage>> authRegister;

        public CustomAuthProviderRegistry()
        {
            this.authRegister = new Dictionary<string, Action<HttpRequestMessage>>();
        }

        public void SetAuthProvider(string name, Action<HttpRequestMessage> act)
        {
            this.authRegister.Add(name, act);
        }

        public Action<HttpRequestMessage> GetAuthProvider(string name)
        {
            this.authRegister.TryGetValue(name, out Action<HttpRequestMessage> value);
            return value;
        }


    }
}
