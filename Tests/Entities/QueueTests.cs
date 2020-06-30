using EventManager.BusinessLogic.Entities;
using EventManager.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Entities
{
    [TestClass]
    public class QueueTests
    {
        private Queue queue;

        [TestInitialize]
        public void SetUp()
        {
            queue = Queue.Instance;
        }

        [TestCategory("Unit")]
        [TestMethod]
        public void Get_QueueGetInstance_ReturnsSameInstance()
        {
            Queue newQueueInstance = Queue.Instance;

            Assert.AreEqual(queue, newQueueInstance);
        }

    }
}
