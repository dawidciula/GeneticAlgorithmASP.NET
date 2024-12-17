using System;
using System.Collections.Generic;

public class FlexibleWorkTimePopulation
{
    // Metoda generująca początkową populację
    public List<int[,]> GenerateInitialPopulation(int populationSize, int workers, int days)
    {
        var random = new Random();
        var population = new List<int[,]>();

        for (int i = 0; i < populationSize; i++)
        {
            var schedule = new int[workers, days];

            // Losowanie zmian dla każdego pracownika
            for (int worker = 0; worker < workers; worker++)
            {
                for (int day = 0; day < days; day++)
                {
                    // Losowanie zmiany: 0 - dzień wolny, 1 - zmiana poranna, 2 - zmiana popołudniowa, 3 - zmiana nocna
                    schedule[worker, day] = random.Next(0, 4); // Losowanie wartości z zakresu 0 do 3
                }
            }

            // Dodanie ograniczeń po wygenerowaniu wstępnej populacji
            ApplyShiftConstraints(schedule);

            population.Add(schedule);
        }

        return population;
    }

    // Metoda do stosowania ograniczeń dotyczących zmian
    private void ApplyShiftConstraints(int[,] schedule)
    {
        int workers = schedule.GetLength(0);
        int days = schedule.GetLength(1);
        
        // Ograniczenie: Niedziela (ostatni dzień tygodnia) jest zawsze dniem wolnym (0)
        for (int worker = 0; worker < workers; worker++)
        {
            schedule[worker, days - 1] = 0; // Ostatni dzień (niedziela) ustawiamy na dzień wolny
        }

        // Możesz tu dodać inne ograniczenia, jeśli będzie to konieczne
        // Na przykład: zmiana nocna nie może występować po porannej itd.
    }
}