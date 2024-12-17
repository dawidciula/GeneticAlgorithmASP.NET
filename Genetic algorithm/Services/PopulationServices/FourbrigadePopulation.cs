using System;
using System.Collections.Generic;
using Genetic_algorithm.Interfaces;

namespace AG.Services
{
    public class FourbrigadePopulation : IPopulationService
    {
        public List<int[,]> GenerateInitialPopulation(int populationSize, int workers, int days)
        {
            var random = new Random();
            var population = new List<int[,]>();

            // For a four-brigade system, the pattern is as follows: 1, 2, 3, 0, 1, 2, 3, 0...
            // 1 = Shift 1, 2 = Shift 2, 3 = Shift 3, 0 = Free day

            // Create initial population
            for (int i = 0; i < populationSize; i++)
            {
                var schedule = new int[workers, days];  // Schedule (rows = workers, columns = days)

                for (int worker = 0; worker < workers; worker++)
                {
                    int workDaysCount = 0;
                    List<int> freeDays = new List<int>();

                    // Assign shifts and days off in a repeating 4-brigade pattern
                    for (int day = 0; day < days; day++)
                    {
                        // Cycle pattern: 1, 2, 3, 0 (1 - Shift 1, 2 - Shift 2, 3 - Shift 3, 0 - Free day)
                        int shift = GetShiftForDay(worker, day);

                        // Make sure there are exactly 5 work days and 2 free days
                        if (shift != 0) // It's a work day
                        {
                            workDaysCount++;
                        }

                        schedule[worker, day] = shift;
                    }

                    // Ensure exactly 5 work days and 2 free days
                    EnsureMinimumWorkDays(schedule, worker, workDaysCount, freeDays, random, days);
                }

                population.Add(schedule);
            }

            return population;
        }

        // Ensure each worker has exactly 5 workdays and 2 free days
        private void EnsureMinimumWorkDays(int[,] schedule, int worker, int workDaysCount, List<int> freeDays, Random random, int daysInWeek)
        {
            // If there are less than 5 working days, assign additional workdays to free days
            while (workDaysCount < 5 && freeDays.Count > 0)
            {
                int dayToWork = freeDays[random.Next(freeDays.Count)];
                freeDays.Remove(dayToWork);

                // Assign a shift to this day
                schedule[worker, dayToWork] = GetShiftForDay(worker, dayToWork);  // Assign a shift (1, 2, or 3)
                workDaysCount++;
            }

            // Ensure exactly 2 free days
            while (freeDays.Count < 2)
            {
                int randomDay;
                do
                {
                    randomDay = random.Next(daysInWeek);  // Ensure this is within valid range
                } while (freeDays.Contains(randomDay));  // Prevent exceeding the days range

                freeDays.Add(randomDay);
                schedule[worker, randomDay] = 0;  // Mark this day as a free day
                workDaysCount--;
            }
        }

        // Helper method to get the shift for a given day in the rotating cycle (1, 2, 3, 0)
        private int GetShiftForDay(int worker, int day)
        {
            // The shift pattern is 1, 2, 3, 0 (repeats every 4 days)
            int[] shiftPattern = { 1, 2, 3, 0 };
            return shiftPattern[day % 4];
        }
    }
}
