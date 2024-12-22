using System.Collections.Generic;

namespace AG.Models
{
    public class EmployeePreferences
    {
        public int WorkerId { get; set; } // Identyfikator pracownika

        public string PreferredMonday { get; set; } = "none"; 
        public string PreferredTuesday { get; set; } = "none"; 
        public string PreferredWednesday { get; set; } = "none"; 
        public string PreferredThursday { get; set; } = "none"; 
        public string PreferredFriday { get; set; } = "none"; 
        public string PreferredSaturday { get; set; } = "none"; 
        public string PreferredSunday { get; set; } = "none"; 

        public int MaxWorkDays { get; set; } = 7; 
        public int MinDaysOff { get; set; } = 0; 

        public string PreferredColleagues { get; set; } = string.Empty; // Preferencje współpracy z innymi pracownikami
    }
}