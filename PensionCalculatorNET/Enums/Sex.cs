namespace PensionCalculatorNET.Enums
{
    public enum Sex
    {
        Male = 1,
        Female,
        PreferNotToSay
    }
    public static class SexExtensions
    {
        public static string GetSexDescription(this Sex sex)
        {
            switch (sex)
            {
                case Sex.Male:
                    return "Male";
                case Sex.Female:
                    return "Female";
                case Sex.PreferNotToSay:
                    return "Prefer not to say";
                default:
                    return string.Empty;
            }
        }
    }
}
