namespace PensionCalculator_API.Models
{
    public class FullProfile
    {
        public HealthProfile? HealthProfile { get; set; }  
        public required UserProfile UserProfile { get; set; }  
    }
}
