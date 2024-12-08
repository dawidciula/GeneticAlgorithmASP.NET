using AG.Models;

namespace Genetic_algorithm.Models;

public class CombinedAlgorithmParameters
{
    public IEnumerable<OptimizationParameters> OptimizationParametersList { get; set; } = new List<OptimizationParameters>();
    public IEnumerable<ScheduleParameters> ScheduleParametersList { get; set; } = new List<ScheduleParameters>();
}