using EventManager.BusinessLogic.Entities;
using EventManager.BusinessLogic.Entities.Auth;
using EventManager.BusinessLogic.Entities.Configuration;
using EventManager.BusinessLogic.Interfaces;
using EventManager.Data;
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
        public static IAuthHandler Create(AuthConfig authConfig)
        {
            IAuthHandler authHandler;
            Enum.TryParse(authConfig.Type, out AuthType authType);
            Log.Debug($"AuthFactory.Create: {authType}");

            switch (authType)
            {
                case AuthType.None:
                    authHandler = new NoneAuth(authConfig);
                    break;
                case AuthType.Basic:
                    authHandler = new BasicAuth(authConfig);
                    break;
                case AuthType.OAuthClientPassword:
                    authHandler = new OAuthClientPassword(authConfig);
                    break;
                default:
                    authHandler = new NoneAuth(authConfig);
                    break;
            }

            return authHandler;
        }

        /// <summary>
        /// Checks if eventSubscriberConf.Endpoint should be concatenated with a BaseURL
        /// and OR add the default Endpoint
        /// </summary>
        /// <param name="eventSubscriberConf"></param>
        /// <param name="subscriberConfig"></param>
        /// <returns></returns>
        public static string Endpoint(EventSubscriberConfiguration eventSubscriberConf, SubscriberConfiguration subscriberConfig)
        {

            AuthConfig authConfig = subscriberConfig.Auth;
            Enum.TryParse(authConfig.Type, out AuthType authType);

            if (authType != AuthType.OAuthClientPassword)
            {
                // do not concatenate
                if (eventSubscriberConf.Endpoint == null)
                {
                    eventSubscriberConf.Endpoint = subscriberConfig.Config.BaseURL + EventManagerConstants.EventReceptionPath;
                }
                else
                {
                    eventSubscriberConf.Endpoint = subscriberConfig.Config.BaseURL + eventSubscriberConf.Endpoint;
                }
            }

            return eventSubscriberConf.Endpoint;
        }
    }
}
