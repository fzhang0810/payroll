using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class EmployeeEntity: TableEntity
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }

        public EmployeeEntity(string fullName, string employeeId = "", string email = "", string phone = "")
        {
            PartitionKey = fullName;
            RowKey = employeeId;
            FullName = fullName;
            Email = email;
            PhoneNumber = phone;
        }

        public EmployeeEntity() { }

        public double AnnualBenefitCosts { get; set; }
        public Int32 numberOfDependents { get; set; }
        public double TakeHomeIncomePerPay { get; set; }
        public double TakeHomeIncomeYearly { get; set; }
    }
}