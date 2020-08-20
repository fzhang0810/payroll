using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public class Calculations
    {
        private const double EmployeeBenefitCost = 1000;
        private const double DependentBenefitCost = 500;
        private const double Paycheck = 2000;
        private const int PayCycle = 26;
        private const double discountBenefit = 0.1;

        public static void CalculatePayroll(EmployeeEntity employee)
        {
            double cost = EmployeeBenefitCost;
            double incomeYearly = Paycheck * PayCycle;
            if (!String.IsNullOrEmpty(employee.FullName) && FirstLettStartsWithA(employee.FullName))
            {
                cost = EmployeeBenefitCost * (1 - discountBenefit);
            }
            employee.AnnualBenefitCosts = cost;
            employee.TakeHomeIncomeYearly = incomeYearly - cost;
            employee.TakeHomeIncomePerPay = System.Math.Round(employee.TakeHomeIncomeYearly / PayCycle, 2);
        }

        public static void CalculatePayrollForUpdates(EmployeeEntity employee)
        {
            double cost = EmployeeBenefitCost;
            double incomeYearly = Paycheck * PayCycle;
            if (!String.IsNullOrEmpty(employee.FullName) && FirstLettStartsWithA(employee.FullName))
            {
                cost = EmployeeBenefitCost * (1 - discountBenefit);
            }
            employee.AnnualBenefitCosts = cost;
            employee.TakeHomeIncomeYearly = incomeYearly - cost;
            employee.TakeHomeIncomePerPay = System.Math.Round(employee.TakeHomeIncomeYearly / PayCycle, 2);
        }

        public static void RemoveDependent(EmployeeEntity employee, DependentsEntity entityDependent)
        {
            employee.numberOfDependents--;
            employee.AnnualBenefitCosts -= entityDependent.AnnualBenefitCosts;
            employee.TakeHomeIncomePerPay = Paycheck * PayCycle - employee.AnnualBenefitCosts;
            employee.TakeHomeIncomePerPay = System.Math.Round((Paycheck * PayCycle - employee.AnnualBenefitCosts) / PayCycle, 2);
        }

       
        public static void AddDependent(EmployeeEntity employee, DependentsEntity dependentEntity)
        {
            employee.numberOfDependents++;
            employee.AnnualBenefitCosts += dependentEntity.AnnualBenefitCosts;
            employee.TakeHomeIncomeYearly = Paycheck * PayCycle - employee.AnnualBenefitCosts;
            employee.TakeHomeIncomePerPay = System.Math.Round((Paycheck * PayCycle - employee.AnnualBenefitCosts) / PayCycle, 2);
        }

        public static bool FirstLettStartsWithA(string letter)
        {
            return letter.Substring(0, 1).ToUpper().Equals("A");
        }

        public static void InitDependentCost(DependentsEntity dependentEntity)
        {
            dependentEntity.AnnualBenefitCosts = DependentBenefitCost;
            if (dependentEntity.FullName.Substring(0, 1).ToUpper().Equals("A"))
            {
                dependentEntity.AnnualBenefitCosts *= 0.9;
            }
        }
    }
}