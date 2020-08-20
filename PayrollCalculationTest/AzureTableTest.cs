using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication2.Services;
using WebApplication2.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace PayrollCalculationTest
{
    [TestClass]
    public class AzureTableTest
    {
        [TestMethod]
        public async void CreateTableTest()
        {
            AzureTable tableStorage = new AzureTable();
            CloudTableClient tableClient = await Config.TableClient();
            await tableStorage.CreateTable(tableClient, "trial1");
            CloudTable table = tableClient.GetTableReference("trail1");
            Assert.IsNotNull(table);
          
        }
    }
}
