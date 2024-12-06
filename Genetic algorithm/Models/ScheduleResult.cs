namespace AG.Models
{
    public class ScheduleResult
    {
        public int[,] BestSchedule { get; set; }
        public double BestFitness { get; set; }
    }
}