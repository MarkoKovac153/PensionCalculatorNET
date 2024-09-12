using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using PensionCalculatorNET.Models;
using PensionCalculatorNET.Interfaces;

namespace PensionCalculatorNET.Services
{

    public class CsvReader : ICsvReaderService  // Implement the interface here
    {
        private readonly ILogger<CsvReader> _logger;
        private readonly string _allDataFilePath;
        private readonly string _longevityDataFilePath;

        public CsvReader(ILogger<CsvReader> logger, IConfiguration configuration)
        {
            _logger = logger;
            _allDataFilePath = configuration["CsvSettings:AllDataFilePath"];
            _longevityDataFilePath = configuration["CsvSettings:LongevityDataFilePath"];
        }

        public int GetLifeExpectancyByCountry(UserProfile userProfile)
        {
            try
            {
                var status = userProfile.Age > (66 - (2028 - DateTime.Now.Year)) ? "Current" : "Future";
                var lines = File.ReadAllLines(_allDataFilePath).Skip(1);

                return lines
                    .Select(line => line.Split(','))
                    .Where(strings => strings[0] == userProfile.Sex ||
                        (strings[0] == "Both Sexes" && userProfile.Sex == "Prefer not to say"))
                    .Select(strings => int.Parse(strings[1]))
                    .FirstOrDefault();
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning("File not found, returning default value");
                return 80;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reading life expectancy data");
                return 80;
            }
        }

        public double GetInflationByCountry(UserProfile userProfile)
        {
            try
            {
                var lines = File.ReadAllLines(_allDataFilePath).Skip(1);

                return lines
                    .Select(line => line.Split(','))
                    .Select(strings => double.Parse(strings[2], System.Globalization.CultureInfo.InvariantCulture))
                    .FirstOrDefault();
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning("File not found, returning default value");
                return 0.0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reading inflation data");
                return 0.0;
            }
        }

        public double GetInterestRateByCountry(UserProfile userProfile)
        {
            try
            {
                var lines = File.ReadAllLines(_allDataFilePath).Skip(1);

                return lines
                    .Select(line => line.Split(','))
                    .Select(strings => double.Parse(strings[5], System.Globalization.CultureInfo.InvariantCulture))
                    .FirstOrDefault();
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning("File not found, returning default value");
                return 1.0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reading interest rate data");
                return 1.0;
            }
        }

        public int GetGovernmentRetirementAgeByCountry(UserProfile userProfile)
        {
            try
            {
                var status = userProfile.Age > (66 - (2028 - DateTime.Now.Year)) ? "Current" : "Future";
                var lines = File.ReadAllLines(_allDataFilePath).Skip(1);

                return lines
                    .Select(line => line.Split(','))
                    .Where(strings => strings[0] == userProfile.Sex ||
                        (strings[0] == "Both Sexes" && userProfile.Sex == "Prefer not to say"))
                    .Where(strings => strings[3] == status)
                    .Select(strings => int.Parse(strings[4]))
                    .FirstOrDefault();
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning("File not found, returning default value");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reading retirement age data");
                return 0;
            }
        }

        public int GetHealthEffectOnLifeExpectancy(HealthProfile healthProfile)
        {
            try
            {
                var lines = File.ReadAllLines(_longevityDataFilePath).Skip(1);

                return lines
                    .Select(line => line.Split(','))
                    .Where(strings =>
                        (strings[0] == "smoking" && healthProfile.IsSmoker) ||
                        (strings[0] == "Pets" && healthProfile.IsPets) ||
                        (strings[0] == "Healthy Eating" && healthProfile.HealthyEating == "yes") ||
                        (strings[0] == "Heavy exercise" && healthProfile.Exercise == "every day"))
                    .Select(strings => int.Parse(strings[1]))
                    .Sum();
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning("File not found, returning default value");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reading health effect data");
                return 0;
            }
        }
    }
}