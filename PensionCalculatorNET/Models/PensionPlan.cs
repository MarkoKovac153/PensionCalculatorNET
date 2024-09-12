namespace PensionCalculator_API.Models
{
    public class PensionPlan
    {
        private double totalPensionPot { get; set; }
        private int governmentRetirementAge { get; set; }
        private int yearsToRetirement{ get; set; }
        private int lifeExpectancy{ get; set; }
        private double annualPensionPayment{ get; set; }
        private double annualPensionWithOtherIncome{ get; set; }
        private double annualPensionAfterTax{ get; set; }
        private double AdjustedForInflation{ get; set; }
        private double interestRate{ get; set; }
        private double inflationRate{ get; set; }
    }
}
