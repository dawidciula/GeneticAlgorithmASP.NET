using AG.Models;

namespace Genetic_algorithm.Models;

public class CombinedAlgorithmParameters
{
    public List<OptimizationParameters> OptimizationParametersList { get; set; } = new List<OptimizationParameters>();
    public List<ScheduleParameters> ScheduleParametersList { get; set; } = new List<ScheduleParameters>();
    public List<EmployeePreferences> EmployeePreferencesList { get; set; } = new List<EmployeePreferences>();
}