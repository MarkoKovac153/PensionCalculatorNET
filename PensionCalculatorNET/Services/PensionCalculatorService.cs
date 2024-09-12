using System;
using System.Globalization;
using Microsoft.Extensions.Logging;
using PensionCalculator_API.Models;  // Replace with actual namespace for UserProfile, HealthProfile, PensionPlan, etc.
using PensionCalculator_API.Interfaces;

namespace PensionCalculator_API.Services
{

    public class PensionCalculatorService
    {
        private readonly ILogger<PensionCalculatorService> _logger;
        private readonly ICsvReaderService _csvReaderService;  // Assuming CsvReader is now a service in C#

        private readonly NumberFormatInfo _currencyFormat;

        public PensionCalculatorService(ILogger<PensionCalculatorService> logger, ICsvReaderService csvReaderService)
        {
            _logger = logger;
            _csvReaderService = csvReaderService;
            _currencyFormat = new NumberFormatInfo { NumberDecimalDigits = 2 };  // Format for currency
        }

        public PensionPlan CalculatePension(UserProfile userProfile, HealthProfile healthProfile)
        {
            var pensionPlan = new PensionPlan
            {
                GovernmentRetirementAge = _csvReaderService.GetGovernmentRetirementAgeByCountry(userProfile)
            };

            if (userProfile.RetirementAge == null)
            {
                userProfile.RetirementAge = pensionPlan.GovernmentRetirementAge;
            }

            RemoveSimpleNulls(userProfile);

            if (healthProfile.IsCompleted())
            {
                CalculateLifeExpectancy(userProfile, pensionPlan, healthProfile);
            }
            else
            {
                CalculateLifeExpectancy(userProfile, pensionPlan);
            }

            pensionPlan.YearsToRetirement = userProfile.RetirementAge.Value - userProfile.Age;

            pensionPlan.InterestRate = userProfile.ExpectedGrowthRate?.ToDouble() ?? _csvReaderService.GetInterestRateByCountry(userProfile);

            CalculateTotalPensionPot(userProfile, pensionPlan);
            AdjustForInflation(userProfile, pensionPlan);
            CalculateAnnualPayment(userProfile, pensionPlan);
            CalculateTax(pensionPlan);
            FormatNumbers(pensionPlan);

            return pensionPlan;
        }

        private void RemoveSimpleNulls(UserProfile userProfile)
        {
            userProfile.CurrentPensionValue ??= 0m;
            userProfile.OtherRetirementIncome ??= 0m;
            userProfile.FinalAnnualSalaryPension ??= 0m;
        }

        private void CalculateTotalPensionPot(UserProfile userProfile, PensionPlan pensionPlan)
        {
            var employerContribution = userProfile.CurrentSalary.Value * userProfile.EmployerContributionRatePercentage.Value / 100;
            var employeeContribution = userProfile.CurrentSalary.Value * userProfile.EmployeeContributionRatePercentage.Value / 100;
            var yearlyContribution = employerContribution + employeeContribution;
            var pensionPot = yearlyContribution + userProfile.CurrentPensionValue.Value;

            for (int i = 0; i < pensionPlan.YearsToRetirement; i++)
            {
                pensionPot *= (1 + pensionPlan.InterestRate / 100);
                pensionPot += yearlyContribution;
            }

            pensionPlan.TotalPensionPot = pensionPot;
        }

        private void CalculateLifeExpectancy(UserProfile userProfile, PensionPlan pensionPlan, HealthProfile healthProfile)
        {
            int lifeExpectancyChange = _csvReaderService.GetHealthEffectOnLifeExpectancy(healthProfile);

            // Married status check
            if (userProfile.MaritalStatus != "SINGLE" && healthProfile.HappyLife == "yes")
            {
                lifeExpectancyChange += 10;
            }
            else if (userProfile.MaritalStatus == "SINGLE" && healthProfile.HappyLife == "yes")
            {
                lifeExpectancyChange += 5;
            }

            if (healthProfile.HappyLife == "no" && lifeExpectancyChange > 0)
            {
                lifeExpectancyChange /= 2;
            }
            else if (healthProfile.HappyLife == "yes" && lifeExpectancyChange > 0)
            {
                lifeExpectancyChange *= 2;
            }

            if (lifeExpectancyChange > 14)
            {
                lifeExpectancyChange = 14;
            }

            pensionPlan.LifeExpectancy = _csvReaderService.GetLifeExpectancyByCountry(userProfile) + lifeExpectancyChange;

            if (pensionPlan.LifeExpectancy <= userProfile.RetirementAge)
            {
                pensionPlan.LifeExpectancy = userProfile.RetirementAge + 1;
            }

            _logger.LogInformation("Life Expectancy: {LifeExpectancy}", pensionPlan.LifeExpectancy);
        }

        private void CalculateLifeExpectancy(UserProfile userProfile, PensionPlan pensionPlan)
        {
            pensionPlan.LifeExpectancy = _csvReaderService.GetLifeExpectancyByCountry(userProfile);
            _logger.LogInformation("Life Expectancy: {LifeExpectancy}", pensionPlan.LifeExpectancy);
        }

        private void CalculateAnnualPayment(UserProfile userProfile, PensionPlan pensionPlan)
        {
            int yearsWithPension = pensionPlan.LifeExpectancy - userProfile.RetirementAge.Value;
            pensionPlan.AnnualPensionPayment = pensionPlan.TotalPensionPot / yearsWithPension;
            pensionPlan.AnnualPensionWithOtherIncome = pensionPlan.AnnualPensionPayment
                + userProfile.OtherRetirementIncome.Value
                + userProfile.FinalAnnualSalaryPension.Value;
        }

        private void AdjustForInflation(UserProfile userProfile, PensionPlan pensionPlan)
        {
            pensionPlan.InflationRate = _csvReaderService.GetInflationByCountry(userProfile);
            pensionPlan.AdjustedForInflation = pensionPlan.TotalPensionPot
                * Math.Pow(1 + (pensionPlan.InflationRate / 100), -pensionPlan.YearsToRetirement);
        }

        private void CalculateTax(PensionPlan pensionPlan)
        {
            double tax = 0.0;
            double pensionIncome = pensionPlan.AnnualPensionWithOtherIncome;
            double personalAllowance = 12570;

            if (pensionIncome > 125140)
            {
                tax = (pensionIncome - 125140) * 0.45 + (125140 - 37700) * 0.4 + 37700 * 0.2;
            }
            else if (pensionIncome > 100000)
            {
                personalAllowance -= (pensionIncome - 100000) / 2;
                tax = (pensionIncome - personalAllowance - 37700) * 0.4 + 37700 * 0.2;
            }
            else if (pensionIncome > 50270)
            {
                tax = (pensionIncome - 50270) * 0.4 + (50270 - 12570) * 0.2;
            }
            else if (pensionIncome > 12570)
            {
                tax = (pensionIncome - 12570) * 0.2;
            }

            pensionPlan.AnnualPensionAfterTax = pensionIncome - tax;
        }

        private void FormatNumbers(PensionPlan pensionPlan)
        {
            pensionPlan.InterestRate = double.Parse(pensionPlan.InterestRate.ToString("F2", _currencyFormat));
            pensionPlan.TotalPensionPot = double.Parse(pensionPlan.TotalPensionPot.ToString("F2", _currencyFormat));
            pensionPlan.AnnualPensionPayment = double.Parse(pensionPlan.AnnualPensionPayment.ToString("F2", _currencyFormat));
            pensionPlan.AnnualPensionWithOtherIncome = double.Parse(pensionPlan.AnnualPensionWithOtherIncome.ToString("F2", _currencyFormat));
            pensionPlan.AdjustedForInflation = double.Parse(pensionPlan.AdjustedForInflation.ToString("F2", _currencyFormat));
            pensionPlan.InflationRate = double.Parse(pensionPlan.InflationRate.ToString("F2", _currencyFormat));
            pensionPlan.AnnualPensionAfterTax = double.Parse(pensionPlan.AnnualPensionAfterTax.ToString("F2", _currencyFormat));
        }
    }
}