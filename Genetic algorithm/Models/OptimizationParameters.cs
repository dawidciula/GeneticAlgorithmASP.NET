namespace AG.Models
{
    public class OptimizationParameters
    {
        public int OptimizationType { get; set; }
        public int PopulationSize { get; set; }
        public int NumberOfWorkers { get; set; }
        public int DaysInWeek { get; set; }
        public double PreferenceWeight { get; set; }
    }
}