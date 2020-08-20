using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Table;
using WebApplication2.Services;

namespace PayrollCalculationTest
{
    [TestClass]
    public class ConfigTest
    {
        [TestMethod]
        public async void TableClientTest()
        {
            CloudTableClient tableClient = await Config.TableClient();
            Assert.IsNotNull(tableClient);
        }
    }
}
