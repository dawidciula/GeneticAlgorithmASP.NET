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

            for (int i = 0; i < populationSize; i++)
            {
                var schedule = new int[days, workers];  // Harmonogram (wiersze = dni, kolumny = pracownicy)
                for (int worker = 0; worker < workers; worker++)
                {
                    int workDaysCount = 0;
                    List<int> freeDays = new List<int>();

                    // Losowanie 2 dni wolnych
                    List<int> randomFreeDays = new List<int>();
                    while (randomFreeDays.Count < 2)
                    {
                        int randomFreeDay = random.Next(0, days);
                        if (!randomFreeDays.Contains(randomFreeDay))
                        {
                            randomFreeDays.Add(randomFreeDay);
                        }
                    }

                    // Przypisanie zmian do dni tygodnia
                    for (int day = 0; day < days; day++)
                    {
                        // Dni wolne
                        if (randomFreeDays.Contains(day) || workDaysCount >= 5)
                        {
                            schedule[day, worker] = 0; // Dzień wolny
                            freeDays.Add(day);
                            continue;
                        }

                        int shift;
                        bool isValidShift;
                        int attempts = 0;

                        // Losowanie zmiany (1: poranna, 2: popołudniowa, 3: nocna)
                        do
                        {
                            shift = random.Next(1, 5);  // Zmiany 1-4 (czterobrygadowy system)
                            isValidShift = IsValidShift(day, shift, schedule, worker);
                            attempts++;
                        }
                        while (!isValidShift && attempts < 10);  // Próbujemy do 10 razy

                        schedule[day, worker] = shift;
                        if (shift != 0) workDaysCount++;
                    }

                    EnsureMinimumWorkDays(schedule, worker, workDaysCount, freeDays, random, days);
                }
                population.Add(schedule);
            }

            return population;
        }

        // Walidacja zmiany: czy zmiana jest dozwolona (np. nie ma nocnej zmiany po dniu wolnym)
        private bool IsValidShift(int day, int shift, int[,] schedule, int worker)
        {
            if (shift == 3 && day > 0 && schedule[day - 1, worker] == 0)  // Jeśli dzień wolny, nie można dać nocnej zmiany
            {
                return false;
            }

            if ((shift == 1 || shift == 2) && day > 0 && schedule[day - 1, worker] == 3)  // Jeśli nocna zmiana poprzedza dzien, nie można dać zmiany porannej lub popołudniowej
            {
                return false;
            }

            return true;
        }

        // Zapewnienie minimalnej liczby dni roboczych (dokładnie 5 dni w tygodniu)
        private void EnsureMinimumWorkDays(int[,] schedule, int worker, int workDaysCount, List<int> freeDays, Random random, int daysInWeek)
        {
            // Gwarantowanie, że pracownik ma dokładnie 5 dni roboczych
            while (workDaysCount < 5 && freeDays.Count > 0)
            {
                int dayToWork = freeDays[random.Next(freeDays.Count)];
                freeDays.Remove(dayToWork);

                // Losowanie zmiany (rano, popołudniu, nocna)
                schedule[dayToWork, worker] = random.Next(1, 5);  // Zmiana 1-4
                workDaysCount++;
            }

            // Gwarantowanie, że pracownik będzie miał dokładnie 2 dni wolne w tygodniu
            while (freeDays.Count < 2)
            {
                int randomDay;
                do
                {
                    randomDay = random.Next(daysInWeek);
                } while (freeDays.Contains(randomDay));

                freeDays.Add(randomDay);
                schedule[randomDay, worker] = 0;  // Dzień wolny
                workDaysCount--;
            }
        }
    }
}
