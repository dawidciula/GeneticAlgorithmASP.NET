using System;

namespace AG.Services
{
    public class FitnessService
    {
        public double CalculateFitness(int[,] schedule)
        {
            double fitness = 0.0;
            int numberOfWorkers = schedule.GetLength(0);
            int daysInWeek = schedule.GetLength(1);
            int targetWorkersPerShift = numberOfWorkers / 4; // 12 pracowników, 4 zmiany -> 12/4 = 3 pracowników na każdą zmianę

            // Sekcja oceniająca równomierność przypisania pracowników do zmian
            for (int day = 0; day < daysInWeek; day++)
            {
                int[] shiftCounts = new int[4]; // Liczba pracowników przypisanych do każdej zmiany (0-3)

                // Zliczanie liczby pracowników przypisanych do każdej zmiany w danym dniu
                for (int worker = 0; worker < numberOfWorkers; worker++)
                {
                    shiftCounts[schedule[worker, day]]++;
                }

                // Przyznawanie punktów za każdą zmianę, która ma dokładnie 3 pracowników
                for (int shift = 0; shift < 4; shift++) // Zmiany są od 0 do 3
                {
                    if (shiftCounts[shift] == targetWorkersPerShift)
                    {
                        fitness += 10; // Dodaj 10 punktów, jeśli zmiana ma dokładnie 3 pracowników
                    }
                }
            }

            // Funkcja zwraca ostateczny wynik
            return fitness;
        }
    }
}