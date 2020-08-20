using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Employee
    {
        public Employee(string name, string email, string phoneNumber, double benefitCost, Int32 numberOfDependents, 
            double incomeYearly, double incomePerPay, string id)
        {
            Dependents = new List<DependentsEntity>();
            AnnualBenefitCosts = benefitCost;
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            NumberOfDependents = numberOfDependents;
            TakeHomeIncomeYearly = incomeYearly;
            TakeHomeIncomeMonthly = incomePerPay;
            EmployeeId = id;
        }

        public string Name { get; set; }

        public string EmployeeId { get; set; }

        public List<DependentsEntity> Dependents { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public double AnnualBenefitCosts { get; set; }
        public Int32 NumberOfDependents { get; set; }

        public double TakeHomeIncomeMonthly { get; set; }

        public double TakeHomeIncomeYearly { get; set; }
    }
}