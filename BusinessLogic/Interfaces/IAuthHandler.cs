using EventManager.BusinessLogic.Entities;
using EventManager.BusinessLogic.Entities.Configuration;
using System.Threading.Tasks;

namespace EventManager.BusinessLogic.Interfaces
{
    public interface IAuthHandler
    {
        Task<bool> SendEvent(Event e, Subscription s);
        bool Valid(Config config, EventSubscriberConfiguration eventSubscriberConfiguration);
    }
}
