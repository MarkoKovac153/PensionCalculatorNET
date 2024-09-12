namespace PensionCalculator_API.Models
{
    public class HealthProfile
    {
        public bool Completed { get; set; } = false;
        public bool Smoker { get; set; } = false;
        public bool Pets { get; set; } = false;
        public string AlcoholConsumption { get; set; } = string.Empty;
        public string Exercise { get; set; } = string.Empty;
        public string HealthyWeight { get; set; } = string.Empty;
        public string HealthyEating { get; set; } = string.Empty;
        public string MentalIllnesses { get; set; } = string.Empty;
        public string HappyLife { get; set; } = string.Empty;
    }
}

