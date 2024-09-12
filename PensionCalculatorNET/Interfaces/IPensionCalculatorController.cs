using Microsoft.AspNetCore.Mvc;
using PensionCalculatorNET.Models;

namespace PensionCalculatorNET.Interfaces
{
    public interface IPensionCalculatorController
    {
        /// <summary>
        /// Calculates the pension based on the provided FullProfile.
        /// </summary>
        /// <returns>An IActionResult containing the result of the pension calculation.</returns>
        IActionResult CalculatePension(FullProfile fullProfile);
    }
}
