namespace PensionCalculatorNET.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UserProfile
    {
        // Needed
        [Range(18, 100, ErrorMessage = "Age should not be less than 18 or greater than 100")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Please provide your assigned sex at birth")]
        public Sex Sex { get; set; }

        [Required(ErrorMessage = "Salary is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Salary cannot be less than 1")]
        public decimal CurrentSalary { get; set; }

        // Removed for now
        // [Required]
        // public Country Country { get; set; }

        [Required(ErrorMessage = "Employer contribution is required")]
        [Range(0, 100, ErrorMessage = "Employer contribution must be between 0 and 100")]
        public decimal EmployerContributionRatePercentage { get; set; }

        [Required(ErrorMessage = "Employee contribution is required")]
        [Range(0, 100, ErrorMessage = "Employee contribution must be between 0 and 100")]
        public decimal EmployeeContributionRatePercentage { get; set; }

        [Required(ErrorMessage = "Marital Status is required")]
        public MaritalStatus MaritalStatus { get; set; }

        // Optional
        [Range(0, double.MaxValue, ErrorMessage = "Final annual salary pension must be zero or a positive value")]
        public decimal? FinalAnnualSalaryPension { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Other retirement income must be zero or a positive value")]
        public decimal? OtherRetirementIncome { get; set; }

        [Range(55, 100, ErrorMessage = "Retirement age must be between 55 and 100")]
        public int? RetirementAge { get; set; }

        [Range(0, 100, ErrorMessage = "Expected growth rate must be between 0 and 100")]
        public decimal? ExpectedGrowthRate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Current pension value must be zero or a positive value")]
        public decimal? CurrentPensionValue { get; set; }
    }

    // Enum placeholders for types
    public enum Sex
    {
        Male,
        Female,
        Other
    }

    public enum MaritalStatus
    {
        Single,
        Married,
        Divorced,
        Widowed
    }

    // You can also add the Country class here if needed in the future.

}
