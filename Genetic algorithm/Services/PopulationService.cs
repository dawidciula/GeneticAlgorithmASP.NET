using System;
using System.Collections.Generic;

namespace AG.Services
{
    public class PopulationService
    {
        public List<int[,]> GenerateInitialPopulation(int populationSize, int workers, int days)
        {
            var random = new Random();
            var population = new List<int[,]>();

            for (int i = 0; i < populationSize; i++)
            {
                var schedule = new int[workers, days];
                for (int worker = 0; worker < workers; worker++)
                {
                    int workDaysCount = 0;
                    List<int> freeDays = new List<int>();
                    bool hadNightShiftBefore = false;

                    int randomFreeDay = random.Next(0, days);

                    for (int day = 0; day < days; day++)
                    {
                        if (day == randomFreeDay || workDaysCount >= 5)
                        {
                            schedule[worker, day] = 0; // Dzień wolny
                            freeDays.Add(day);
                            continue;
                        }

                        int shift;
                        bool isValidShift;
                        int attempts = 0;

                        do
                        {
                            shift = random.Next(1, 4); // 1: rano, 2: popołudnie, 3: noc
                            isValidShift = IsValidShift(day, shift, schedule, worker, hadNightShiftBefore);
                            if (shift == 3) hadNightShiftBefore = true;
                            attempts++;
                        }
                        while (!isValidShift && attempts < 10);

                        schedule[worker, day] = isValidShift ? shift : 0;
                        if (isValidShift) workDaysCount++;
                    }

                    EnsureMinimumWorkDays(schedule, worker, workDaysCount, freeDays, random);
                }
                population.Add(schedule);
            }

            return population;
        }

        private bool IsValidShift(int day, int shift, int[,] schedule, int worker, bool hadNightShiftBefore)
        {
            if (shift == 3 && day > 0 && !hadNightShiftBefore && schedule[worker, day - 1] != 0)
            {
                return false;
            }

            if ((shift == 1 || shift == 2) && day > 0 && schedule[worker, day - 1] == 3)
            {
                return false;
            }

            return true;
        }

        private void EnsureMinimumWorkDays(int[,] schedule, int worker, int workDaysCount, List<int> freeDays, Random random)
        {
            int daysInWeek = schedule.GetLength(1);

            while (workDaysCount < 5 && freeDays.Count > 0)
            {
                int dayToWork = freeDays[random.Next(freeDays.Count)];
                freeDays.Remove(dayToWork);

                schedule[worker, dayToWork] = random.Next(1, 4);
                workDaysCount++;
            }

            while (freeDays.Count < 2)
            {
                int randomDay;
                do
                {
                    randomDay = random.Next(daysInWeek);
                } while (freeDays.Contains(randomDay));

                freeDays.Add(randomDay);
                schedule[worker, randomDay] = 0;
                workDaysCount--;
            }
        }
    }
}