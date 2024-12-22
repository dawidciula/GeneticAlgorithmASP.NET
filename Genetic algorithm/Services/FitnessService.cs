using System;
using System.Collections.Generic;
using Genetic_algorithm.Models;

namespace AG.Services
{
    public class FitnessService
    {
        public double CalculateFitness(
            int[,] schedule, 
            List<int[]> employeePreferences, 
            ScheduleParameters scheduleParameters)
        {
            double fitness = 0.0;
            int numberOfWorkers = schedule.GetLength(0);
            int daysInWeek = schedule.GetLength(1);

            // Parametry zmian z ScheduleParameters
            int morningShiftWorkers = scheduleParameters.MorningShiftWorkers;
            int afternoonShiftWorkers = scheduleParameters.AfternoonShiftWorkers;
            int nightShiftWorkers = scheduleParameters.NightShiftWorkers;

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

                // Dodawanie punktów za zgodność liczby pracowników ze zmianami
                if (morningShiftCount == morningShiftWorkers)
                    fitness += 10; // Idealna liczba pracowników na porannej zmianie
                else
                    fitness -= Math.Abs(morningShiftCount - morningShiftWorkers); // Kara za odchylenie

                if (afternoonShiftCount == afternoonShiftWorkers)
                    fitness += 10; // Idealna liczba pracowników na popołudniowej zmianie
                else
                    fitness -= Math.Abs(afternoonShiftCount - afternoonShiftWorkers); // Kara za odchylenie

                if (nightShiftCount == nightShiftWorkers)
                    fitness += 10; // Idealna liczba pracowników na nocnej zmianie
                else
                    fitness -= Math.Abs(nightShiftCount - nightShiftWorkers); // Kara za odchylenie

                // Sprawdzanie, czy reszta pracowników ma dzień wolny
                int totalAssignedWorkers = morningShiftCount + afternoonShiftCount + nightShiftCount;
                if (totalAssignedWorkers > morningShiftWorkers + afternoonShiftWorkers + nightShiftWorkers)
                {
                    fitness -= (totalAssignedWorkers - (morningShiftWorkers + afternoonShiftWorkers + nightShiftWorkers)) * 5; 
                    // Kara za nadmiar pracowników przypisanych do zmian
                }
            }

            // Ocena zgodności harmonogramu z preferencjami pracowników
            for (int worker = 0; worker < numberOfWorkers; worker++)
            {
                var preferences = employeePreferences[worker];
                if (preferences == null) continue;

                for (int day = 0; day < daysInWeek; day++)
                {
                    int assignedShift = schedule[worker, day];
                    int preferredShift = preferences[day];

                    if (preferredShift == assignedShift)
                    {
                        fitness += 5; // Zgodność preferencji
                    }
                    else if (preferredShift == 0 && assignedShift != 0)
                    {
                        fitness -= 3; // Brak dnia wolnego zgodnego z preferencjami
                    }
                }
            }

            // Reguła nocnych zmian
            for (int worker = 0; worker < numberOfWorkers; worker++)
            {
                for (int day = 1; day < daysInWeek; day++)
                {
                    if (schedule[worker, day - 1] == 3 && schedule[worker, day] != 3 && schedule[worker, day] != 0)
                    {
                        fitness -= 10; // Naruszenie reguły nocnych zmian
                    }
                }
            }

            return fitness;
        }
    }
}
