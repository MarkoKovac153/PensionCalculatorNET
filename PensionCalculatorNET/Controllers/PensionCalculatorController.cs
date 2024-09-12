using Microsoft.AspNetCore.Mvc;
using PensionCalculator_API.Models;

namespace PensionCalculator_API.Controllers
{
    [Route("api")]
    [ApiController]
    public class PensionCalculatorController : Controller
    {
        [HttpPost("calculate")]
        public IActionResult CalculatePension([FromBody] FullProfile fullProfile)
        {
            // Check for validation errors
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values.SelectMany(v => v.Errors)
                                                     .Select(e => e.ErrorMessage)
                                                     .FirstOrDefault();
                throw new MissingPropertyException($"Has errors: {errorMessage}");
            }

            // Check if age is greater than retirement age
            if (fullProfile.UserProfile.RetirementAge.HasValue && fullProfile.UserProfile.Age > fullProfile.UserProfile.RetirementAge)
            {
                throw new InvalidAgeException("Age cannot be greater than retirement age");
            }

            // Check if retirement age is not provided and age is greater than 65
            if (!fullProfile.UserProfile.RetirementAge.HasValue && fullProfile.UserProfile.Age > 65)
            {
                throw new InvalidAgeException("Age cannot be greater than government retirement age");
            }

            // Convert contribution rates to percentages
            fullProfile.UserProfile.EmployeeContributionRatePercentage =
                fullProfile.UserProfile.EmployeeContributionRatePercentage / 100;

            fullProfile.UserProfile.EmployerContributionRatePercentage =
                fullProfile.UserProfile.EmployerContributionRatePercentage / 100;

            // Call the pension calculator service and return the result
            var result = _pensionCalculatorService.CalculatePension(fullProfile.UserProfile, fullProfile.HealthProfile);

            return Ok(result);
        }

    }
}
