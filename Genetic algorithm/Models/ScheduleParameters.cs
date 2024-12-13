namespace Genetic_algorithm.Models;

public enum WorkRegime
{
    Fourbrigade,
    FlexibleWorkTime
}

public class ScheduleParameters
{
    public int Id { get; set; }
    public WorkRegime WorkRegime { get; set; }
    public int NumberOfWorkers { get; set; }
    public int DaysInWeek { get; set; }
    public int NumberOfWeeks { get; set; }
}