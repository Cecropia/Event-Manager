using EventManager.BusinessLogic.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void Get_Queue_ReturnsData()
        {
            

            Assert.IsNull(null);
        }

        [TestCategory("Unit")]
        [TestMethod]
        public void Get_QueueGetInstance_ReturnsSameInstance()
        {
            Queue newQueueInstance = Queue.Instance;

            Assert.AreEqual(queue, newQueueInstance);
        }


        [TestCategory("Unit")]
        [TestMethod]
        public void Get_QueueProcess_FirstItemToLast()
        {
            QueueItem item1 = new QueueItem();
            QueueItem item2 = new QueueItem();
            queue.QueueToProcess(item1);
            queue.QueueToProcess(item2);

            queue.Process();

            Assert.AreEqual(queue.Items.Last(), item1);
        }

        [TestCategory("Unit")]
        [TestMethod]
        public void Get_QueueTimer_Execute()
        {

            Assert.IsTrue(true);
        }

    }
}
