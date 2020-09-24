using EventManager.BusinessLogic.Entities.Configuration;
using EventManager.Data;
using EventManager.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class EventManagerMiddlewareTests
    {
        public DefaultHttpContext httpContext;
        public EventManagerMiddleware eventManagerMiddleware { get; set; }
        public string data;

        [TestInitialize]
        public void SetUp()
        {
            httpContext = new DefaultHttpContext(); // or mock a `HttpContext`

            data = @"
                {
                  'Name'  : 'careers_salesforce_after_skill_created',
                  'Payload':[
                    {'item1':'value1'},
                    {'item2':'value2'}
                  ],
                  'Timestamp': '2020-05-22T21:28:06.496Z',
                  'ExtraParams': { 'name': 'value'}, 
                }
            ";


        }




    }
}
