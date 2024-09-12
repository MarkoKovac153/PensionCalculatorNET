using PensionCalculatorNET.Interfaces;
using PensionCalculatorNET.Models;
using System.Globalization;

namespace PensionCalculatorNET.Interfaces
{
    public interface IPensionCalculatorService
    {
        PensionPlan CalculatePension(UserProfile userProfile, HealthProfile healthProfile);
        void RemoveSimpleNulls(UserProfile userProfile);
        void CalculateTotalPensionPot(UserProfile userProfile, PensionPlan pensionPlan);
        void CalculateLifeExpectancy(UserProfile userProfile, PensionPlan pensionPlan, HealthProfile healthProfile);
        void CalculateLifeExpectancy(UserProfile userProfile, PensionPlan pensionPlan);
        void CalculateAnnualPayment(UserProfile userProfile, PensionPlan pensionPlan);
        void AdjustForInflation(UserProfile userProfile, PensionPlan pensionPlan);
        void CalculateTax(PensionPlan pensionPlan);
        void FormatNumbers(PensionPlan pensionPlan);

    }
}
