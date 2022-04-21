using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace IndustrialTesting
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            int k = 1;

            Assert.AreEqual(k, k);
        }
    }
}