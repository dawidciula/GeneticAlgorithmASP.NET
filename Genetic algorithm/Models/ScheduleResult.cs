namespace AG.Models
{
    public class ScheduleResult
    {
        public int[,] BestSchedule { get; set; }
        public double BestFitness { get; set; }
        public List<string> UnmetPreferences { get; set; } // Dodajemy listę niespełnionych preferencji
    }
}