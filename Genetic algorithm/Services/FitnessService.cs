using System;
using System.Collections.Generic;
using System.Linq;
using Genetic_algorithm.Models;

namespace AG.Services
{
    public class FitnessService
    {
        public double CalculateFitness(int[,] schedule, List<int[]> employeePreferences, ScheduleParameters scheduleParameters)
{
    double fitness = 0.0;
    int numberOfWorkers = schedule.GetLength(0);
    int daysInWeek = schedule.GetLength(1);

    // Parametry zmian z ScheduleParameters
    int morningShiftWorkers = scheduleParameters.MorningShiftWorkers;
    int afternoonShiftWorkers = scheduleParameters.AfternoonShiftWorkers;
    int nightShiftWorkers = scheduleParameters.NightShiftWorkers;

    // Wagi dla różnych aspektów
    double shiftBalanceWeight = 1.0;
    double preferenceWeight = 1.5;
    double colleaguePreferenceWeight = 1.5;
    double maxWorkDaysWeight = 2.0;
    double minDaysOffWeight = 1.5;
    double nightShiftPenaltyWeight = 2.0;
    double defaultWorkDaysWeight = 2.0;  // Nowa waga dla dni pracy

    // Sekcja oceniająca liczbę pracowników przypisanych do każdej zmiany
    for (int day = 0; day < daysInWeek; day++)
    {
        int morningShiftCount = 0;
        int afternoonShiftCount = 0;
        int nightShiftCount = 0;

        // Zliczanie pracowników przypisanych do poszczególnych zmian
        for (int worker = 0; worker < numberOfWorkers; worker++)
        {
            int assignedShift = schedule[worker, day];
            switch (assignedShift)
            {
                case 1: // Poranna zmiana
                    morningShiftCount++;
                    break;
                case 2: // Popołudniowa zmiana
                    afternoonShiftCount++;
                    break;
                case 3: // Nocna zmiana
                    nightShiftCount++;
                    break;
            }
        }

        // Nagroda/punkty za równomierny rozdział pracowników
        fitness += shiftBalanceWeight * (Math.Abs(morningShiftCount - morningShiftWorkers) == 0 ? 20 : -Math.Abs(morningShiftCount - morningShiftWorkers) * 2);
        fitness += shiftBalanceWeight * (Math.Abs(afternoonShiftCount - afternoonShiftWorkers) == 0 ? 20 : -Math.Abs(afternoonShiftCount - afternoonShiftWorkers) * 2);
        fitness += shiftBalanceWeight * (Math.Abs(nightShiftCount - nightShiftWorkers) == 0 ? 20 : -Math.Abs(nightShiftCount - nightShiftWorkers) * 2);

        // Sprawdzanie, czy reszta pracowników ma dzień wolny
        int totalAssignedWorkers = morningShiftCount + afternoonShiftCount + nightShiftCount;
        if (totalAssignedWorkers > morningShiftWorkers + afternoonShiftWorkers + nightShiftWorkers)
        {
            fitness -= (totalAssignedWorkers - (morningShiftWorkers + afternoonShiftWorkers + nightShiftWorkers)) * 10;
        }
    }

    // Ocena zgodności harmonogramu z preferencjami pracowników
    for (int worker = 0; worker < numberOfWorkers; worker++)
    {
        var preferences = employeePreferences[worker];
        if (preferences.All(p => p == -1)) continue; // Pomijamy pracowników z brakującymi preferencjami

        for (int day = 0; day < daysInWeek; day++)
        {
            int assignedShift = schedule[worker, day];
            int preferredShift = preferences[day];

            if (preferredShift == -1) continue; // Pomijamy brak preferencji dla konkretnego dnia

            if (preferredShift == assignedShift)
            {
                fitness += preferenceWeight; // Zgodność preferencji
            }
            else if (preferredShift == 0 && assignedShift != 0)
            {
                fitness -= preferenceWeight * 2; // Kara za przypisanie zmiany, kiedy preferencja to dzień wolny
            }
            else if (preferredShift != 0 && assignedShift == 0)
            {
                fitness -= preferenceWeight * 2; // Kara za przypisanie dnia wolnego, kiedy preferencja to zmiana
            }
        }
    }

    // Ocena preferencji współpracowników
    for (int worker = 1; worker <= numberOfWorkers; worker++)
    {
        var preferences = employeePreferences[worker - 1]; // Dostosowanie indeksu do tabeli (zaczynamy od 0)
        var preferredColleagues = preferences.Skip(9).ToList(); // Współpracownicy są dodawani po 8. indeksie

        if (preferredColleagues.Count == 0 || preferredColleagues.All(c => c == -1))
            continue; // Brak preferowanych współpracowników

        // Sprawdzanie, czy preferowani współpracownicy pracują razem
        for (int day = 0; day < daysInWeek; day++)
        {
            int assignedShift = schedule[worker - 1, day]; // Dostosowanie indeksu `worker` do tablicy `schedule`

            foreach (var colleagueIndex in preferredColleagues)
            {
                if (colleagueIndex != -1)
                {
                    // Pracownik i współpracownik muszą pracować razem
                    if (assignedShift == schedule[colleagueIndex - 1, day]) // Dostosowanie indeksu `colleagueIndex` do tablicy `schedule`
                    {
                        fitness += colleaguePreferenceWeight; // Współpracownicy pracują razem, dodajemy punkty
                    }
                    else
                    {
                        fitness -= colleaguePreferenceWeight / 2; // Współpracownicy nie pracują razem, odejmujemy punkty
                    }
                }
            }
        }
    }

    // Reguła nocnych zmian (bez zmian)
    for (int worker = 0; worker < numberOfWorkers; worker++)
    {
        for (int day = 1; day < daysInWeek; day++)
        {
            if (schedule[worker, day - 1] == 3 && schedule[worker, day] != 3 && schedule[worker, day] != 0)
            {
                fitness -= nightShiftPenaltyWeight; // Naruszenie reguły nocnych zmian
            }
        }
    }

    // Nowa sekcja oceny zgodności z MaxWorkDays i MinDaysOff
    for (int worker = 0; worker < numberOfWorkers; worker++)
    {
        var preferences = employeePreferences[worker];

        // Jeśli MaxWorkDays lub MinDaysOff są ustawione na -1, pomijamy te parametry
        int maxWorkDays = preferences[7] != -1 ? preferences[7] : int.MaxValue; // Jeśli brak preferencji, ustawiamy jako maksymalną liczbę dni
        int minDaysOff = preferences[8] != -1 ? preferences[8] : 0; // Jeśli brak preferencji, ustawiamy jako 0

        int workDaysCount = 0;
        int daysOffCount = 0;

        for (int day = 0; day < daysInWeek; day++)
        {
            int assignedShift = schedule[worker, day];

            if (assignedShift != 0) // Jeśli przypisana jest zmiana (nie dzień wolny)
            {
                workDaysCount++;
            }
            else
            {
                daysOffCount++;
            }
        }

        // Nagroda za zbliżenie do MaxWorkDays
        if (workDaysCount == maxWorkDays)
        {
            fitness += maxWorkDaysWeight; // Punkty za dokładnie odpowiednią liczbę dni pracy
        }
        else if (workDaysCount > maxWorkDays)
        {
            fitness -= (workDaysCount - maxWorkDays) * 5; // Kara za zbyt dużo dni pracy
        }

        // Nagroda za zbliżenie do MinDaysOff
        if (daysOffCount == minDaysOff)
        {
            fitness += minDaysOffWeight; // Punkty za dokładnie odpowiednią liczbę dni wolnych
        }
        else if (daysOffCount < minDaysOff)
        {
            fitness -= (minDaysOff - daysOffCount) * 5; // Kara za za mało dni wolnych
        }

        // Premiowanie harmonogramów, które przypisują 5 dni pracy, chyba że w preferencjach wybrano inaczej
        int defaultWorkDays = 5;
        if (workDaysCount == defaultWorkDays && preferences[7] == -1) // Jeśli preferencja nie jest ustawiona, traktujemy to jako domyślną wartość
        {
            fitness += defaultWorkDaysWeight; // Punkty za domyślne przydzielenie 5 dni pracy
        }
    }

    return fitness;
}

    }
}
