using EventManager.BusinessLogic.Entities;
using EventManager.BusinessLogic.Entities.Auth;
using EventManager.BusinessLogic.Entities.Config;
using EventManager.BusinessLogic.Interfaces;
using Serilog;
using System;

namespace EventManager.BusinessLogic.Factories
{
    public class AuthFactory
    {
        /// <summary>
        /// Retrieves the appropiate Authorization
        /// </summary>
        /// <param name="auth">Object representing the EventManager.Auth section inside AppSettings.json</param>
        /// <returns>Authorization as an IAuthHandler</returns>
        public static IAuthHandler Create(AuthConfig auth)
        {
            IAuthHandler authHandler = null;
            Enum.TryParse(auth.Type, out AuthType authType);
            Log.Debug($"AuthFactory.Create: {authType}");

            switch (authType)
            {
                case AuthType.None:
                    authHandler = new NoneAuth(auth);
                    break;
                case AuthType.Basic:
                    authHandler = new BasicAuth(auth);
                    break;
                case AuthType.OAuth:
                    break;
                default:
                    authHandler = new NoneAuth(auth);
                    break;
            }

            return authHandler;
        }
    }
}
