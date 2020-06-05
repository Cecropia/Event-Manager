using EventManager.Data;
using EventManager.Middleware;
using Microsoft.AspNetCore.Http;
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


            eventManagerMiddleware = new EventManagerMiddleware((httpContext) => { return Task.CompletedTask; });
        }

        [TestCategory("Unit")]
        [TestMethod]
        public async Task Get_WhenInvokeAsync_ReturnsData()
        {
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));
            httpContext.Request.Body = stream;

            await eventManagerMiddleware.InvokeAsync(httpContext);

            Assert.IsNull(null);
        }

        [TestCategory("Unit")]
        [TestMethod]
        public async Task Get_WhenInvokeAsyncWithPathAndMethod_ReturnsData()
        {
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));
            httpContext.Request.Path = EventManagerConstants.EventReceptionPath;
            httpContext.Request.Method = "POST";
            httpContext.Request.Body = stream;

            await eventManagerMiddleware.InvokeAsync(httpContext);

            Assert.AreEqual(httpContext.Response.StatusCode, 200);
        }


    }
}
