using Microsoft.VisualStudio.TestTools.UnitTesting;
using DepthSenseWarper.DepthExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthSenseWarper.DepthExport.Tests
{
    [TestClass()]
    public class NewReceivedDataTests
    {
        //[AssemblyInitialize()]
        //public static void Initialize(TestContext context)
        //{
        //    Warper.Initialize();
        //}

        //[AssemblyCleanup()]
        //public static void Cleanup()
        //{
        //    Warper.Finalization();
        //}

        [TestMethod()]
        public void GetDataTest()
        {
            NewReceivedData data = new NewReceivedData();

            while (!data.IsNodeSet())
                System.Threading.Thread.Sleep(1000);

            data.GetData();

            Assert.IsNotNull(data.data.confidenceMap, "confidenceMap");
            Assert.IsNull(data.data.phaseMap, "phaseMap");
            Assert.IsNotNull(data.data.depthMap, "depthMap");
            Assert.IsNull(data.data.depthMapFloatingPoint, "depthMapFloatingPoint");
            Assert.IsNotNull(data.data.vertices, "vertices");
            Assert.IsNull(data.data.verticesFloatingPoint, "verticesFloatingPoint");
            Assert.IsNotNull(data.data.uvMap, "uvMap");
            Assert.AreEqual((UInt64)0, data.data.timeOfCapture, "timeOfCapture");
            Assert.AreEqual((UInt64)0, data.data.timeOfArrival, "timeOfArrival");
            Assert.AreEqual((UInt32)0, data.data.droppedSampleCount, "droppedSampleCount");
            Assert.AreEqual((UInt32)0, data.data.cumulativeDroppedSampleCount, "cumulativeDroppedSampleCount");
        }

        [TestMethod()]
        public void IsNodeSetTest()
        {
            NewReceivedData data = new NewReceivedData();
            Assert.IsTrue(data.IsNodeSet());
        }
    }
}