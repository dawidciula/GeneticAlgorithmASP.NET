using System;
using System.Collections.Generic;

namespace AG.Services
{
    public class FitnessService
    {
        public double CalculateFitness(int[,] schedule, List<int[]> employeePreferences, double preferenceWeight)
        {
            double fitness = 0.0;
            int numberOfWorkers = schedule.GetLength(0);
            int daysInWeek = schedule.GetLength(1);
            int targetWorkersPerShift = numberOfWorkers / 3;

            for (int worker = 0; worker < numberOfWorkers; worker++)
            {
                if (employeePreferences[worker] != null)
                {
                    for (int day = 0; day < daysInWeek; day++)
                    {
                        int shift = schedule[worker, day];
                        int preference = employeePreferences[worker][day];
                        if (shift == preference)
                        {
                            fitness += 10 * preferenceWeight;
                        }
                    }
                }
            }

            for (int day = 0; day < daysInWeek; day++)
            {
                int[] shiftCounts = new int[4];
                for (int worker = 0; worker < numberOfWorkers; worker++)
                {
                    shiftCounts[schedule[worker, day]]++;
                }

                for (int shift = 1; shift <= 3; shift++)
                {
                    fitness += 10 - Math.Abs(shiftCounts[shift] - targetWorkersPerShift);
                }
            }
            
            //Sekcja oceniające harmonogramy ze względu na równomierne obsadzenie pracowników na zmianach
            for (int day = 0; day < daysInWeek; day++)
            {
                int morningShiftCount = 0;
                int afternoonShiftCount = 0;
                int nightShiftCount = 0;

                for (int employee = 0; employee < numberOfWorkers; employee++)
                {
                    int shift = schedule[employee, day];
                    if (shift == 1) morningShiftCount++;
                    else if (shift == 2) afternoonShiftCount++;
                    else if (shift == 3) nightShiftCount++;
                }

                double morningPenalty = Math.Abs(morningShiftCount - targetWorkersPerShift);
                double afternoonPenalty = Math.Abs(afternoonShiftCount - targetWorkersPerShift);
                double nightPenalty = Math.Abs(nightShiftCount - targetWorkersPerShift);

                fitness += (10 - morningPenalty) * 3;
                fitness += (10 - afternoonPenalty) * 3;
                fitness += (10 - nightPenalty) * 3;
            }

            return fitness;
        }
    }
}