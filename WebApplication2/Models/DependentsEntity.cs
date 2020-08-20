using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class DependentsEntity: TableEntity
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }

        public DependentsEntity(string fullName, int id, string dependantName, string email, string phone)
        {
            PartitionKey = fullName;
            RowKey = id.ToString();
            FullName = dependantName;
            Email = email;
            PhoneNumber = phone;
            //AnnualBenefitCosts = 500;
            //if (FullName.Substring(0, 1).ToUpper().Equals("A"))
            //{
            //    AnnualBenefitCosts *= 0.9;
            //}
        }

        public DependentsEntity() { }

        public double AnnualBenefitCosts { get; set; }
    }
}