using EventManager.BusinessLogic.Entities;
using System.Threading.Tasks;

namespace EventManager.BusinessLogic.Interfaces
{
    public interface IAuthHandler
    {
        Task<bool> SendEvent(Event e, Subscription s);
    }
}
