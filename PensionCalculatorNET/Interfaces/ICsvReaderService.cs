using System;

using PensionCalculator_API.Models;  // Replace with your actual namespace for UserProfile and HealthProfile

namespace PensionCalculator_API.Interfaces
{
    public interface ICsvReaderService
    {
        /// <summary>
        /// Retrieves the life expectancy based on the user's profile and country data.
        /// </summary>
        /// <returns>Life expectancy as an integer.</returns>
        int GetLifeExpectancyByCountry(UserProfile userProfile);

        /// <summary>
        /// Retrieves the inflation rate based on the user's country.
        /// </summary>
        /// <returns>Inflation rate as a double.</returns>
        double GetInflationByCountry(UserProfile userProfile);

        /// <summary>
        /// Retrieves the interest rate based on the user's country.
        /// </summary>
        /// <returns>Interest rate as a double.</returns>
        double GetInterestRateByCountry(UserProfile userProfile);

        /// <summary>
        /// Retrieves the government-set retirement age based on the user's country.
        /// </summary>
        /// <returns>Government retirement age as an integer.</returns>
        int GetGovernmentRetirementAgeByCountry(UserProfile userProfile);

        /// <summary>
        /// Retrieves the effect of the user's health profile on life expectancy.
        /// </summary>
        /// <returns>Effect on life expectancy as an integer.</returns>
        int GetHealthEffectOnLifeExpectancy(HealthProfile healthProfile);
    }
}

