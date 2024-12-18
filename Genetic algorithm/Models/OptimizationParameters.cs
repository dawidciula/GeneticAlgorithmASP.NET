namespace AG.Models
{
    public enum OptimizationType
    {
        Roulette = 1, // Ruletka
        Tournament = 2 // Turniej
    }

    public class OptimizationParameters
    {
        public int Id { get; set; }
        public OptimizationType OptimizationType { get; set; }
        public int PopulationSize { get; set; }
        public double PreferenceWeight { get; set; }
        public double MutationFrequency { get; set; }
        public int NumberOfParents { get; set; }
        public double ElitePercentage { get; set; }
        //public int MaxGenerations { get; set; }  // Liczba generacji
        //public int MaxStagnation { get; set; }  // Maksymalna liczba generacji bez poprawy
        //public int NumberOfCrossoverPoints { get; set; }  // Liczba punktów krzyżowania

    }
}