using System;
using System.Collections.Generic;
using System.Net.Http;

namespace EventManager.BusinessLogic.Entities
{
    /// <summary>
    /// The CustomAuthProviderRegistry is just an encapsulation for a Dictionary
    /// that associates string with the Action that can be invoked by auth adapters.
    /// </summary>
    public class CustomAuthProviderRegistry
    {
        private Dictionary<string, Action<HttpRequestMessage, Subscription>> authRegister;

        public CustomAuthProviderRegistry()
        {
            this.authRegister = new Dictionary<string, Action<HttpRequestMessage, Subscription>>();
        }

        /// <summary>
        /// Add an auth provider method to the registry using the specified name
        /// </summary>
        /// <param name="name">Name with which the method will be registered</param>
        /// <param name="act">Action that can then be called by auth adapters</param>
        public void AddAuthProvider(string name, Action<HttpRequestMessage, Subscription> act)
        {
            this.authRegister.Add(name, act);
        }

        /// <summary>
        /// Obtains the callable auth provider method with the specified name. Returns
        /// null if there is no element with `name`.
        /// </summary>
        /// <param name="name">Name of the action we want to get</param>
        /// <returns>The action with the specified `name` or `null` if said name doesn't exist</returns>
        public Action<HttpRequestMessage, Subscription> GetAuthProvider(string name)
        {
            this.authRegister.TryGetValue(name, out Action<HttpRequestMessage, Subscription> value);
            return value;
        }


    }
}
