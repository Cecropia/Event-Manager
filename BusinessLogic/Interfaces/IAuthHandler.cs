using EventManager.BusinessLogic.Entities;
using EventManager.BusinessLogic.Entities.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace EventManager.BusinessLogic.Interfaces
{
    /// <summary>
    /// Every AuthHandler implements a different authentication mechanism, and is an abstraction layer
    /// that is in charge of sending Events to Subscribers respecting the Subscriber configuration.
    /// </summary>
    public interface IAuthHandler
    {
        /// <summary>
        /// This method is used to have an auth handler send the specified Event using the
        /// configuration provided in the Subscription. 
        /// </summary>
        /// <param name="e">The Event we want to send</param>
        /// <param name="s">The Subscription that specifies how the event is to be sent</param>
        /// <returns>The response message obtained after sending the event</returns>
        Task<HttpResponseMessage> SendEvent(Event e, Subscription s);

        /// <summary>
        /// This method is used to validate the configuration provided for this Subscriber. The idea
        /// is that this method fail if the Config or EventSubscriberConfiguration don't contain all
        /// the necessary information for this AuthProvider to perform its _auth scheme_.
        /// </summary>
        /// <param name="config">The Config we want to check</param>
        /// <param name="eventSubscriberConfiguration">The EventSubscriberConfiguration we want to check</param>
        /// <returns>A bool specifying whether the configuration is valid or not</returns>
        bool Valid(Config config, EventSubscriberConfiguration eventSubscriberConfiguration);
    }
}
