using System.ComponentModel.DataAnnotations;

namespace AG.Models
{
    public class AlgorithmInputModel
    {
        [Required]
        public OptimizationParameters OptimizationParameters { get; set; }

        [Required]
        public EmployeePreference EmployeePreference { get; set; }
    }
}