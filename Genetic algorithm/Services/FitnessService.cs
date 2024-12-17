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
            int targetWorkersPerShift = numberOfWorkers / 3;

            // Sekcja oceniająca równomierność przypisania pracowników do zmian
            for (int day = 0; day < daysInWeek; day++)
            {
                int[] shiftCounts = new int[4]; // Liczba pracowników przypisanych do każdej zmiany (0-3)

                // Zliczanie liczby pracowników przypisanych do każdej zmiany w danym dniu
                for (int worker = 0; worker < numberOfWorkers; worker++)
                {
                    shiftCounts[schedule[worker, day]]++;
                }

                // Dodawanie punktów za idealne przypisanie pracowników do zmian
                for (int shift = 1; shift <= 3; shift++)
                {
                    if (shiftCounts[shift] == targetWorkersPerShift)
                    {
                        fitness += 5; // Dodaj punkty za idealne przypisanie do zmiany
                    }
                }
            }

            // Dodatkowe punkty, jeśli w każdej zmianie każdego dnia jest dokładnie 3 pracowników
            bool isBalanced = true;
            for (int day = 0; day < daysInWeek; day++)
            {
                int[] shiftCounts = new int[4]; // Liczba pracowników przypisanych do każdej zmiany (0-3)

                // Zliczanie liczby pracowników przypisanych do każdej zmiany w danym dniu
                for (int worker = 0; worker < numberOfWorkers; worker++)
                {
                    shiftCounts[schedule[worker, day]]++;
                }

                // Sprawdzenie, czy każda zmiana ma dokładnie 3 pracowników
                for (int shift = 0; shift <= 3; shift++)
                {
                    if (shiftCounts[shift] != 3)
                    {
                        isBalanced = false;
                        break;
                    }
                }

                if (!isBalanced)
                    break;
            }

            if (isBalanced)
            {
                fitness += 10; // Dodaj 10 punktów, jeśli warunek jest spełniony
            }

            return fitness;
        }
    }
}
