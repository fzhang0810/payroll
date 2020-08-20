using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication2.Services;

namespace PayrollCalculationTest
{
    [TestClass]
    public class CalculationsTest
    {
        [TestMethod]
        public void TestFirstLettStartsWithA()
        {
            Assert.AreEqual(Calculations.FirstLettStartsWithA("Alex Wang"), true);
        }

        [TestMethod]
        public void TestFirstLettStartsNotWithA()
        {
            Assert.AreEqual(Calculations.FirstLettStartsWithA("Test Wang"), false);
        }
    }
}
