using DepthSenseWarper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DepthSenseWarper.Tests
{
    [TestClass()]
    public class WarperTests
    {
        [AssemblyInitialize()]
        public static void Initialize(TestContext context)
        {
            Warper.Initialize();
        }

        [AssemblyCleanup()]
        public static void Cleanup()
        {
            Warper.Finalization();
        }

        [TestMethod()]
        public void DoNothingTest()
        {
            Warper.DoNothing();
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SizeofDepthExportNewSampleReceivedDataPtrTest()
        {
            int size = Warper.SizeofStruct("DepthExport::NewSampleReceivedData");
            Assert.AreEqual(Marshal.SizeOf(typeof(DepthExport.NewSampleReceivedDataPtr)), size);
        }

        [TestMethod()]
        public void SizeofColorExportNewSampleReceivedDataPtrTest()
        {
            int size = Warper.SizeofStruct("ColorExport::NewSampleReceivedData");
            Assert.AreEqual(Marshal.SizeOf(typeof(ColorExport.NewSampleReceivedDataPtr)), size);
        }

        [TestMethod()]
        public void SizeofDepthSenseVertexTest()
        {
            int size = Warper.SizeofStruct("DepthSense::Vertex");
            Assert.AreEqual(Marshal.SizeOf(typeof(Vertex)), size);
        }

        [TestMethod()]
        public void SizeofDepthSenseFPVertexTest()
        {
            int size = Warper.SizeofStruct("DepthSense::FPVertex");
            Assert.AreEqual(Marshal.SizeOf(typeof(FPVertex)), size);
        }

        [TestMethod()]
        public void SizeofDepthSenseUVTest()
        {
            int size = Warper.SizeofStruct("DepthSense::UV");
            Assert.AreEqual(Marshal.SizeOf(typeof(UV)), size);
        }
    }
}