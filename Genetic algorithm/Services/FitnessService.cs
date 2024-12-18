using System;
using System.Collections.Generic;

namespace AG.Services
{
    public class FitnessService
    {
        public double CalculateFitness(int[,] schedule, List<int[]> employeePreferences)
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

            // Ocena zgodności harmonogramu z preferencjami pracowników
            for (int worker = 0; worker < numberOfWorkers; worker++)
            {
                // Sprawdzamy, czy preferencje pracownika są null
                var preferences = employeePreferences[worker];
                if (preferences == null) continue; // Jeśli brak preferencji, pomijamy tego pracownika

                // Pracownik z null preferencjami nie jest brany pod uwagę
                for (int day = 0; day < daysInWeek; day++)
                {
                    int assignedShift = schedule[worker, day];
                    int preferredShift = preferences[day];

                    if (preferredShift == assignedShift)
                    {
                        // Przyznaj punkty, jeśli przypisana zmiana odpowiada preferencji pracownika
                        fitness += 5; // 5 punktów za zgodność z preferencjami
                    }
                    else if (preferredShift == 0 && assignedShift != 0)
                    {
                        // Jeśli preferowany dzień wolny (0) i pracownik jest przypisany do zmiany, odejmujemy punkty
                        fitness -= 3; // -3 punkty za brak dnia wolnego
                    }
                }
            }

            // Sprawdzanie, czy po nocnej zmianie (3) przypisana jest tylko nocna zmiana (3) lub dzień wolny (0)
            for (int worker = 0; worker < numberOfWorkers; worker++)
            {
                for (int day = 1; day < daysInWeek; day++) // Zaczynamy od dnia 1, bo nie ma poprzedniego dnia dla dnia 0
                {
                    if (schedule[worker, day - 1] == 3 && schedule[worker, day] != 3 && schedule[worker, day] != 0)
                    {
                        // Jeśli poprzedni dzień to nocna zmiana (3) a obecny dzień nie jest ani nocną zmianą (3), ani dniem wolnym (0),
                        // odejmujemy punkty (np. -10 punktów za złamanie reguły).
                        fitness -= 10; // -10 punktów za naruszenie zasady
                    }
                }
            }

            // Funkcja zwraca ostateczny wynik
            return fitness;
        }
    }
}