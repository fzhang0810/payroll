using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Payroll
    {
        public double TotalBenefitCosts { get; set; }

        public List<Employee> EmployeePayroll { get; set; }

        public Payroll()
        {
            
            EmployeePayroll = new List<Employee>();
        }
    }
}