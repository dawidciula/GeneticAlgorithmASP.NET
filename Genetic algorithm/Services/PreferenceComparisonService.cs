using System;
using System.Collections.Generic;
using System.Linq;

namespace AG.Services
{
    public class PreferenceComparisonService
    {
        public List<string> ComparePreferences(
            List<int[]> employeePreferences, 
            int[,] generatedSchedule)
        {
            var unmetPreferences = new List<string>();

            // Pobieramy liczbę pracowników i dni z generatedSchedule
            int numEmployees = generatedSchedule.GetLength(0); // Liczba pracowników
            int numDays = generatedSchedule.GetLength(1); // Liczba dni (w tygodniu)

            // Sprawdzamy, czy liczba pracowników w preferences jest zgodna
            if (employeePreferences.Count != numEmployees)
            {
                unmetPreferences.Add($"Błąd: Liczba pracowników w preferencjach ({employeePreferences.Count}) nie zgadza się z harmonogramem ({numEmployees}).");
                return unmetPreferences; // Zwracamy błąd i przerywamy dalszą analizę
            }

            for (int employee = 0; employee < numEmployees; employee++)
            {
                // Sprawdzamy, czy liczba dni w preferencjach pracownika jest zgodna z liczbą dni w harmonogramie
                int[] preferences = employeePreferences[employee];

                // Sprawdzamy, czy tablica preferencji zawiera co najmniej 9 elementów
                if (preferences.Length < 9)
                {
                    unmetPreferences.Add($"Błąd: Preferencje dla pracownika {employee + 1} mają nieprawidłową liczbę dni. (Preferencje: {preferences.Length}, Oczekiwano co najmniej 9)");
                    continue; // Przechodzimy do następnego pracownika
                }

                // Dni tygodnia (pierwsze 7 elementów)
                int[] workDaysPreferences = new int[7];
                Array.Copy(preferences, 0, workDaysPreferences, 0, 7);

                // maxWorkDays (8. element) i minFreeDays (9. element)
                int maxWorkDays = preferences[7];
                int minFreeDays = preferences[8];

                // Sprawdzamy preferencje współpracowników, które mogą być po 9. indeksie
                var preferredColleagues = preferences.Skip(9).ToList(); // Zbieramy współpracowników z listy preferencji

                int workDaysCount = 0;
                int daysOffCount = 0;

                // Sprawdzamy dni tygodnia
                for (int day = 0; day < 7; day++) // 0-6 to dni tygodnia
                {
                    int preference = workDaysPreferences[day];
                    int assignedShift = generatedSchedule[employee, day];

                    // Liczymy dni pracy i dni wolne tylko w przypadku, gdy preferencja nie wynosi -1
                    if (assignedShift != 0)
                    {
                        workDaysCount++; // Zliczamy dni pracy, jeśli przypisana zmiana to 1, 2, lub 3
                    }
                    else
                    {
                        daysOffCount++; // Zliczamy dni wolne, jeśli przypisana zmiana to 0
                    }

                    // Sprawdzamy, czy preferencja została spełniona
                    if (!IsPreferenceMet(preference, assignedShift))
                    {
                        unmetPreferences.Add(
                            $"Pracownik {employee + 1}, Dzień {day + 1}: Zmiana {preference} nie mogła zostać przypisana, przydzielona zmiana: {assignedShift}");
                    }
                }

                // Sprawdzamy dni pracy i dni wolne względem maxWorkDays i minFreeDays
                if (maxWorkDays != -1 && workDaysCount > maxWorkDays)
                {
                    unmetPreferences.Add($"Pracownik {employee + 1} ma więcej dni pracy niż {maxWorkDays}. Liczba dni pracy: {workDaysCount}");
                }

                if (minFreeDays != -1 && daysOffCount < minFreeDays)
                {
                    unmetPreferences.Add($"Pracownik {employee + 1} ma za mało dni wolnych. Minimalna liczba dni wolnych to {minFreeDays}. Liczba dni wolnych: {daysOffCount}");
                }

                // Sprawdzamy preferencje współpracowników
                if (preferredColleagues.Count > 0)
                {
                    // Sprawdzamy, czy preferowani współpracownicy pracują razem
                    for (int day = 0; day < 7; day++) // 0-6 to dni tygodnia
                    {
                        int assignedShift = generatedSchedule[employee, day];

                        foreach (var colleagueIndex in preferredColleagues)
                        {
                            if (colleagueIndex != -1)
                            {
                                // Indeks współpracownika zaczyna się od 1, więc musimy to uwzględnić
                                int colleagueShift = generatedSchedule[colleagueIndex - 1, day];

                                if (assignedShift == colleagueShift)
                                {
                                    // Pracownicy pracują razem, nic nie robimy
                                }
                                else
                                {
                                    unmetPreferences.Add($"Pracownik {employee + 1}, Dzień {day + 1}: Preferowany współpracownik {colleagueIndex} nie pracuje razem z pracownikiem {employee + 1}.");
                                }
                            }
                        }
                    }
                }
            }

            return unmetPreferences;
        }

        private bool IsPreferenceMet(int preference, int assignedShift)
        {
            // -1 oznacza brak preferencji, a 0 oznacza dzień wolny
            if (preference == -1) return true; // Brak preferencji, zawsze spełnione
            if (preference == 0) return assignedShift == 0; // Dzień wolny preferowany, spełniony tylko jeśli shift = 0
            return preference == assignedShift; // Preferencja spełniona, jeśli zgadza się ze zmianą
        }
    }
}
