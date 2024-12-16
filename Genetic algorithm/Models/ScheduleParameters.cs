namespace Genetic_algorithm.Models;

public enum WorkRegime
{
    FlexibleWorkTime = 1,
    Fourbrigade = 2,
}

public class ScheduleParameters
{
    public int Id { get; set; }
    public WorkRegime WorkRegime { get; set; }
    public int NumberOfWorkers { get; set; }
    public int DaysInWeek { get; set; }
    public int NumberOfWeeks { get; set; }
}