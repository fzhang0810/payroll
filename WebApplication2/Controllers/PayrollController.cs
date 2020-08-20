
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Http;
using WebApplication2.Models;
using Swashbuckle.Swagger.Annotations;
using System.Net.Http;
using Microsoft.WindowsAzure.Storage.Table;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    /// <summary>
    /// Payroll Calculation Apis
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [RoutePrefix("api")]
    public class PayrollController : ApiController
    {

        /// <summary>
        /// Create table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.Created, Description = "Creates Table")]
        [HttpPost, Route("createtable/tablename/{tableName}")]
        public async Task<IHttpActionResult> CreateTable(string tableName)
        {
            try
            {
                CloudTableClient tableClient = await Config.TableClient();
                AzureTable tableStorage = new AzureTable();
                await tableStorage.CreateTable(tableClient, tableName);
                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }



        /// <summary>
        /// Get a employee
        /// </summary>
        /// <param name="employeeName"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.Created, Description = "Get a employee")]
        [HttpGet, Route("employee/{employeeName}/{employeeId}")]
        public async Task<Employee> GetEmployee(string employeeName, string employeeId)
        {
            try
            {
                CloudTableClient tableClient = await Config.TableClient();
                AzureTable tableStorage = new AzureTable();
             
                TableResult retrievedResult = tableStorage.RetrieveEntity(tableClient, Constants.EMPLOYEE_TABLE, employeeName, employeeId);
                CloudTable tableDependent = tableClient.GetTableReference(Constants.DEPENDENT_TABLE);
               
                return tableStorage.GetEmployeeEntity(tableDependent, retrievedResult, employeeName);
            }
            catch (Exception)
            {
                throw;
            }

        }


        /// <summary>
        /// Add a Dependent
        /// </summary>
        /// <param name="employeeName"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.Created, Description = "Add a Dependent")]
        [HttpPost, Route("employee/{employeeName}/{employeeId}/dependent")]
        public async Task<Employee> AddDependent(string employeeName, string employeeId, Dependent dependent)
        {
            try
            {
                CloudTableClient tableClient = await Config.TableClient();
                AzureTable tableStorage = new AzureTable();
                CloudTable table = tableClient.GetTableReference(Constants.EMPLOYEE_TABLE);
                table.CreateIfNotExists();

                CloudTable tableDependent = tableClient.GetTableReference(Constants.DEPENDENT_TABLE);
                tableDependent.CreateIfNotExists();
                
                tableStorage.InsertDependentEntity(table, tableDependent, dependent, employeeName, employeeId);

                TableOperation retrieveOperation = TableOperation.Retrieve<EmployeeEntity>(employeeName, employeeId);

                TableResult retrievedResult = table.Execute(retrieveOperation);
                
                return tableStorage.GetEmployeeEntity(tableDependent, retrievedResult, employeeName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Create employee
        /// </summary>
        /// <param name="employeeName"></param>
        /// <param name="employeeId"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.Created, Description = "Creates Employee")]
        [HttpPost, Route("employee/name/{employeeName}/id/{employeeId}")]
        public async Task<IHttpActionResult> CreateEmployee(string employeeName, string employeeId, Employee employee)
        {
            try
            {
                CloudTableClient tableClient = await Config.TableClient();
                CloudTable table = tableClient.GetTableReference(Constants.EMPLOYEE_TABLE);
                table.CreateIfNotExists();

                EmployeeEntity newAddedEmployee = new EmployeeEntity(employeeName, employeeId, employee.Email, employee.PhoneNumber);

                Calculations.CalculatePayroll(newAddedEmployee);
                // Create the TableOperation that inserts the customer entity.
                var insertOperation = TableOperation.Insert(newAddedEmployee);
                
                // Execute the insert operation.
                table.Execute(insertOperation);

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        /// <summary>
        /// Update employee    Assume employee does not have any dependent
        /// </summary>
        /// <param name="employeeName"></param>
        /// <param name="employeeId"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Description = " Update employee")]
        [HttpPut, Route("employee/update/name/{employeeName}/id/{employeeId}")]
        public async Task<IHttpActionResult> UpdateEmployee(string employeeName, string employeeId, Employee employee)
        {
            try
            {
                CloudTableClient tableClient = await Config.TableClient();
                AzureTable tableStorage = new AzureTable();
                CloudTable table = tableClient.GetTableReference(Constants.EMPLOYEE_TABLE);
                TableOperation retrieveOperation = TableOperation.Retrieve<EmployeeEntity>(employeeName, employeeId);

                // Execute the insert operation.
                TableResult retrievedResult = table.Execute(retrieveOperation);

                EmployeeEntity updateEntity = (EmployeeEntity)retrievedResult.Result;

                if (updateEntity != null)
                {
                    updateEntity.numberOfDependents = employee.NumberOfDependents;
                    updateEntity.FullName = employee.Name;
                    updateEntity.Email = employee.Email;
                    updateEntity.PhoneNumber = employee.PhoneNumber;
                    Calculations.CalculatePayrollForUpdates(updateEntity);
                    TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(updateEntity);
                    await table.ExecuteAsync(insertOrReplaceOperation);
                }
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        /// <summary>
        /// Delete Dependents
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Description = "Deletes Dependents")]
        [HttpPost, Route("delete/dependents/employname/{employeeName}/{employeeId}")]
        public async Task<Employee> DeleteDependents(string[] dependents, string employeeName, string employeeId)
        {
            try
            {
                CloudTableClient tableClient = await Config.TableClient();
                AzureTable tableStorage = new AzureTable();
                CloudTable table = tableClient.GetTableReference(Constants.EMPLOYEE_TABLE);
                table.CreateIfNotExists();

                CloudTable tableDependent = tableClient.GetTableReference(Constants.DEPENDENT_TABLE);
                tableDependent.CreateIfNotExists();
                TableOperation retrieveOperation = TableOperation.Retrieve<EmployeeEntity>(employeeName, employeeId);

                TableResult retrievedResult = table.Execute(retrieveOperation);


                tableStorage.DeleteDependents(table, tableDependent, retrievedResult, employeeName, dependents);

                return tableStorage.GetEmployeeEntity(tableDependent, retrievedResult, employeeName);
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
