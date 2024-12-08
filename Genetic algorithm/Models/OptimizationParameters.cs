namespace AG.Models
{

    public class OptimizationParameters
    {
        public int Id { get; set; }
        public int OptimizationType { get; set; }
        public int PopulationSize { get; set; }
        public double PreferenceWeight { get; set; }
        public double MutationFrequency { get; set; }
        public int NumberOfParents { get; set; }
        public double ElitePercentage { get; set; }
    }
}