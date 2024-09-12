using Microsoft.AspNetCore.Mvc;
using PensionCalculatorNET.Interfaces;
using PensionCalculatorNET.Models;
using System;

namespace PensionCalculator_API.Controllers
{
    [Route("api")]
    [ApiController]
    public class PensionCalculatorController : Controller, IPensionCalculatorController
    {
        private readonly IPensionCalculatorService _pensionCalculatorService;

        public PensionCalculatorController(IPensionCalculatorService pensionCalculatorService)
        {
            _pensionCalculatorService = pensionCalculatorService;
        }

        [HttpPost("calculate")]
        public IActionResult CalculatePension([FromBody] FullProfile fullProfile)
        {
            try
            {
                // Check for validation errors
                if (!ModelState.IsValid)
                {
                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors)
                                                         .Select(e => e.ErrorMessage)
                                                         .FirstOrDefault();
                    throw new ArgumentException($"Invalid input: {errorMessage}");
                }

                // Check if age is greater than retirement age
                if (fullProfile.UserProfile.RetirementAge.HasValue && fullProfile.UserProfile.Age > fullProfile.UserProfile.RetirementAge)
                {
                    throw new ArgumentOutOfRangeException(nameof(fullProfile.UserProfile.Age), "Age cannot be greater than retirement age.");
                }

                // Check if retirement age is not provided and age is greater than 65
                if (!fullProfile.UserProfile.RetirementAge.HasValue && fullProfile.UserProfile.Age > 65)
                {
                    throw new ArgumentOutOfRangeException(nameof(fullProfile.UserProfile.Age), "Age cannot be greater than government retirement age.");
                }

                // Convert contribution rates to percentages
                fullProfile.UserProfile.EmployeeContributionRatePercentage /= 100;
                fullProfile.UserProfile.EmployerContributionRatePercentage /= 100;

                // Call the pension calculator service and return the result
                var result = _pensionCalculatorService.CalculatePension(fullProfile.UserProfile, fullProfile.HealthProfile);

                return Ok(result);
            }
            catch (Exception ex)  // Generic catch for any other exceptions
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
