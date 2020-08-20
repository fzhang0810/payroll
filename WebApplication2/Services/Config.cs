using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;
using System.IO;
using System.Configuration;

namespace WebApplication2.Services
{
    /// <summary>
    /// config connection string class
    /// </summary>
    public class Config
    {
        /// <summary>
        /// get connection string
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static async Task<string> GetConnectionString(string Key)
        {
            try
            {
                return await Task.Run(() =>
                {
                    return ConfigurationManager.AppSettings[Key];
                });
            }
            catch (Exception)
            {

                return null;
            }
        }

        /// <summary>
        /// return table client
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static async Task<CloudTableClient> TableClient()
        {

            // Retrieve storage account from connection string.
            try
            {
                return await Task.Run(() => {

                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
                    // Create the table client.
                    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                    return tableClient;

                });

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}