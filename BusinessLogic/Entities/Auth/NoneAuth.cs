using EventManager.BusinessLogic.Entities.Config;
using EventManager.BusinessLogic.Extensions;
using EventManager.BusinessLogic.Interfaces;
using Serilog;
using System.Threading.Tasks;

namespace EventManager.BusinessLogic.Entities.Auth
{
    class NoneAuth : IAuthHandler
    {
        public NoneAuth(AuthConfig authConfi)
        {

        }
        public async Task<bool> SendEvent(Event e, Subscription s)
        {
            Log.Debug("NoneAuth.SendEvent");

            return await HttpClientExtension.MakeCallRequest(e.Payload, s.Method, s.EndPoint);
        }
    }
}
