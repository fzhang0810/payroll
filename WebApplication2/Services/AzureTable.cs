using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class AzureTable
    {

        ///Retrie entity from table
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableClient"></param>
        /// <param name="tableName"></param>
        /// <param name="employeeName"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public TableResult RetrieveEntity(CloudTableClient tableClient, string tableName, string employeeName, string employeeId)
        {
            try
            {
                if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(employeeName) && !string.IsNullOrEmpty(employeeId))
                {
                    CloudTable table = tableClient.GetTableReference(tableName);
                    table.CreateIfNotExists();

                    TableOperation retrieveOperation = TableOperation.Retrieve<EmployeeEntity>(employeeName, employeeId);

                    TableResult retrievedResult = table.Execute(retrieveOperation);
                    return retrievedResult;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///Get employee entity
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableDependent"></param>
        /// <param name="retrievedResult"></param>
        /// <param name="employeeName"></param>
        /// <returns></returns>
        public Employee GetEmployeeEntity(CloudTable tableDependent, TableResult retrievedResult, string employeeName)
        {
            try
            {
                if (!string.IsNullOrEmpty(employeeName))
                {
                    var query = new TableQuery<DependentsEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, employeeName));

                    var queryResult = tableDependent.ExecuteQuery(query).ToList();
                    EmployeeEntity employeeRes = (EmployeeEntity)retrievedResult.Result;
                    Employee res = new Employee(employeeRes.FullName, employeeRes.Email, employeeRes.PhoneNumber,
                        employeeRes.AnnualBenefitCosts, employeeRes.numberOfDependents, employeeRes.TakeHomeIncomeYearly, 
                        employeeRes.TakeHomeIncomePerPay, employeeRes.RowKey);
                    res.Dependents = queryResult;
                    return res;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///Insert a dependent
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableDependent"></param>
        /// <param name="dependent"></param>
        /// <param name="employeeName"></param>
        /// <returns></returns>
        public void InsertDependentEntity(CloudTable table, CloudTable tableDependent, Dependent dependent, string employeeName, string employeeId)
        {
            try
            {
                if (!string.IsNullOrEmpty(employeeName))
                {
                    int UUID = new Random().Next(1000, 9999);
                    DependentsEntity dependentEntity = new DependentsEntity(employeeName, UUID, dependent.FullName, dependent.Email, dependent.PhoneNumber);
                    Calculations.InitDependentCost(dependentEntity);
                    var insertOperation = TableOperation.Insert(dependentEntity);
                    tableDependent.Execute(insertOperation);

                    TableOperation retrieveOperation = TableOperation.Retrieve<EmployeeEntity>(employeeName, employeeId);

                    TableResult retrievedResult = table.Execute(retrieveOperation);
                    EmployeeEntity updateEntity = (EmployeeEntity)retrievedResult.Result;
                    if (updateEntity != null)
                    {
                        Calculations.AddDependent(updateEntity, dependentEntity);
                      
                        TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(updateEntity);

                        // Execute the operation.
                        table.Execute(insertOrReplaceOperation);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///Delete employee entity
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableDependent"></param>
        /// <param name="retrievedResult"></param>
        /// <param name="employeeName"></param>
        /// <returns></returns>
        public void DeleteDependents(CloudTable table, CloudTable tableDependent, TableResult retrievedResult, string employeeName, string[] dependents)
        {
            try
            {
                EmployeeEntity updateEntity = (EmployeeEntity)retrievedResult.Result;
                foreach (string rowKey in dependents)
                {
                    DependentsEntity entity = new DependentsEntity
                    {
                        PartitionKey = employeeName,
                        RowKey = rowKey,
                        ETag = "*"
                    };
                    TableOperation retrieveOperationDependent = TableOperation.Retrieve<DependentsEntity>(employeeName, rowKey);
                    TableResult retrievedResultDependent = tableDependent.Execute(retrieveOperationDependent);
                    DependentsEntity entityDependent = (DependentsEntity)retrievedResultDependent.Result;

                    Calculations.RemoveDependent(updateEntity, entityDependent);
                 
                    TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(updateEntity);

                    // Execute the operation.
                    table.Execute(insertOrReplaceOperation);

                    TableOperation deleteOperation = TableOperation.Delete(entity);
                    tableDependent.Execute(deleteOperation);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///add entity to table
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableClient"></param>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> InsertEntity(CloudTableClient tableClient, string tableName = "", TableEntity entity = null)
        {
            try
            {
                bool isOperationSuccessful = false;
                if (!string.IsNullOrEmpty(tableName))
                {
                    CloudTable table = tableClient.GetTableReference(tableName);
                    // Create the TableOperation object that inserts the customer entity.
                    TableOperation insertOperation = TableOperation.Insert(entity);
                    await table.ExecuteAsync(insertOperation);
                    isOperationSuccessful = true;

                }
                return isOperationSuccessful;
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///delete entity to table
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableClient"></param>
        /// <param name="tableName"></param>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public async Task<bool> DeleteEntity(CloudTableClient tableClient, string tableName = "", string partitionKey = "", string rowKey = "")
        {
            try
            {
                bool isOperationSuccessful = false;
                if (!string.IsNullOrEmpty(tableName))
                {
                    CloudTable table = tableClient.GetTableReference(tableName);
                    TableOperation retrieveOperation = TableOperation.Retrieve<TableEntity>(partitionKey, rowKey);
                    // Execute the operation.
                    TableResult retrievedResult = table.Execute(retrieveOperation);
                    TableEntity deleteEntity = (TableEntity)retrievedResult.Result;
                    TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                    await table.ExecuteAsync(deleteOperation);
                    isOperationSuccessful = true;
                }
                return isOperationSuccessful;
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///create  multiple table
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableClient"></param>
        /// <param name="tableList"></param>
        /// <returns></returns>
        public async Task<bool> CreateMultipleTable(CloudTableClient tableClient, List<string> tableList = null)
        {
            try
            {
                bool isOperationSuccessful = false;
                foreach (string table in tableList)
                {
                    CloudTable singleTable = tableClient.GetTableReference(table);
                    isOperationSuccessful = await singleTable.CreateIfNotExistsAsync();
                }
                return isOperationSuccessful;

            }
            catch (Exception)
            {
                throw;
            }
        }

        ///create single table
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableClient"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<CloudTable> CreateTable(CloudTableClient tableClient, string tableName = "")
        {
            try
            {
                CloudTable singleTable = tableClient.GetTableReference(tableName);
                await singleTable.CreateIfNotExistsAsync();
                return singleTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //delete table
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableClient"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<bool> DeleteTable(CloudTableClient tableClient, string tableName = "")
        {
            try
            {
                bool isOperationSuccessful = false;
                CloudTable singleTable = tableClient.GetTableReference(tableName);
                if (!string.IsNullOrEmpty(tableName))
                {
                    isOperationSuccessful = await singleTable.DeleteIfExistsAsync();
                }
                return isOperationSuccessful;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //delete multiple table
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableClient"></param>
        /// <param name="tableList"></param>
        /// <returns></returns>
        public async Task<bool> DeleteMultipleTable(CloudTableClient tableClient, List<string> tableList = null)
        {
            try
            {
                bool isOperationSuccessful = false;
                foreach (string table in tableList)
                {
                    CloudTable singleTable = tableClient.GetTableReference(table);
                    isOperationSuccessful = await singleTable.DeleteIfExistsAsync();
                }
                return isOperationSuccessful;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}