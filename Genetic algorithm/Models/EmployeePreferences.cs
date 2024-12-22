using System;

namespace AG.Models
{
    public class Shifts
    {
        public string Monday { get; set; } = "none";
        public string Tuesday { get; set; } = "none";
        public string Wednesday { get; set; } = "none";
        public string Thursday { get; set; } = "none";
        public string Friday { get; set; } = "none";
        public string Saturday { get; set; } = "none";
        public string Sunday { get; set; } = "none";
    }

    public class EmployeePreferences
    {
        //public int WorkerId { get; set; } // Identyfikator pracownika

        public Shifts Shifts { get; set; } = new Shifts(); // Zmienione na obiekt Shifts

        public int MaxWorkDays { get; set; } = 7;
        public int MinDaysOff { get; set; } = 0;
        public string PreferredColleagues { get; set; } = string.Empty; // Preferencje współpracy z innymi pracownikami
    }
}