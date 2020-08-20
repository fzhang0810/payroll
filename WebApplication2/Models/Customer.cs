using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class CustomerEntity : TableEntity
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }

        public CustomerEntity(string fullName, string dependantName = "")
        {
            PartitionKey = fullName;
            RowKey = dependantName;
            FullName = fullName;
        }

        public CustomerEntity() { }

        public double AnnualBenefitCosts { get; set; }
        public Int32 numberOfDependents { get; set; }
    }
}