using System;
using System.Collections.Generic;

namespace AG.Services
{
    public class MutationService
    {
        public void PerformMutation(List<int[,]> population, Random random, int workers, int days, double mutationFrequency)
        {
            foreach (var schedule in population)
            {
                if (random.NextDouble() < mutationFrequency)
                {
                    int numberOfMutations = (int)Math.Max(1, mutationFrequency * workers * days);

                    for (int i = 0; i < numberOfMutations; i++)
                    {
                        int worker = random.Next(workers);
                        int day = random.Next(days);

                        // Pomijamy mutację w dni wolne globalnie (np. niedziela)
                        if (IsGlobalFreeDay(day))
                            continue;

                        // Zachowujemy oryginalne ograniczenia
                        int originalShift = schedule[worker, day];
                        int newShift;
                        bool isValidShift;
                        int attempts = 0;

                        do
                        {
                            newShift = random.Next(0, 4); // 0: wolne, 1: rano, 2: popołudnie, 3: noc
                            isValidShift = IsValidShift(day, newShift, schedule, worker);
                            attempts++;
                        } while ((!isValidShift || newShift == originalShift) && attempts < 10);

                        // Jeśli mutacja jest ważna, wprowadź ją
                        if (isValidShift)
                        {
                            schedule[worker, day] = newShift;

                            // Zapewnij zgodność z ograniczeniami dziennymi
                            EnsureMinimumWorkDays(schedule, worker, random);
                        }
                    }
                }
            }
        }

        private bool IsValidShift(int day, int shift, int[,] schedule, int worker)
        {
            // Dni wolne globalne (np. niedziela) mogą mieć tylko 0 (dzień wolny)
            if (IsGlobalFreeDay(day))
                return shift == 0;

            // Nocna zmiana nie może nastąpić po dniu pracy
            if (shift == 3 && day > 0 && schedule[worker, day - 1] != 0)
                return false;

            // Poranna lub popołudniowa zmiana nie może nastąpić po nocnej
            if ((shift == 1 || shift == 2) && day > 0 && schedule[worker, day - 1] == 3)
                return false;

            return true;
        }

        private void EnsureMinimumWorkDays(int[,] schedule, int worker, Random random)
        {
            int daysInWeek = schedule.GetLength(1);
            List<int> freeDays = new List<int>();
            int workDaysCount = 0;

            for (int day = 0; day < daysInWeek; day++)
            {
                if (schedule[worker, day] == 0)
                {
                    freeDays.Add(day);
                }
                else
                {
                    workDaysCount++;
                }
            }

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

        private bool IsGlobalFreeDay(int day)
        {
            // Zakładamy, że niedziela to ostatni dzień tygodnia (day == 6)
            const int SundayIndex = 6;
            return day == SundayIndex;
        }
    }
}
