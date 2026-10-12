using System.Globalization;

namespace RtClassifier;

public static class Report
{
    private const double PercentFactor = 100.0;

    private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");

    public static void Print(InputData data)
    {
        List<RealtimeTask> tasks = data.Tasks;
        SystemParameters parameters = data.Parameters;

        Console.WriteLine($"Система: {data.SystemName}");
        Console.WriteLine($"Процессоров: {parameters.CpuCount}, сеть: {Localization.Presence(parameters.HasNetwork)}. Времена указаны в миллисекундах.");
        Console.WriteLine();

        PrintTable(tasks);
        Console.WriteLine();
        PrintConclusion(tasks, parameters);
    }

    private static void PrintTable(List<RealtimeTask> tasks)
    {
        string[] headers = { "Задача", "Класс", "C", "D", "T", "L", "Выполнимо", "Тип дедлайна" };
        List<string[]> rows = tasks.Select(task => new[]
        {
            task.Name,
            Localization.Name(task.Hardness),
            Num(task.C),
            Num(task.D),
            Num(task.T),
            Num(TaskAnalysis.Slack(task)),
            Localization.YesNo(TaskAnalysis.IsFeasible(task)),
            Localization.Name(TaskAnalysis.GetDeadlineType(task))
        }).ToList();

        int[] widths = headers.Select((header, i) => Math.Max(header.Length, rows.Max(row => row[i].Length))).ToArray();
        bool[] rightAligned = { false, false, true, true, true, true, false, false };

        string headerLine = FormatRow(headers, widths, rightAligned);
        Console.WriteLine(headerLine);
        Console.WriteLine(new string('-', headerLine.Length));
        foreach (string[] row in rows)
        {
            Console.WriteLine(FormatRow(row, widths, rightAligned));
        }
    }

    private static string FormatRow(string[] cells, int[] widths, bool[] rightAligned)
    {
        string[] formatted = cells.Select((cell, i) =>
            i == cells.Length - 1 ? cell : rightAligned[i] ? cell.PadLeft(widths[i]) : cell.PadRight(widths[i])).ToArray();
        return string.Join(" | ", formatted);
    }

    private static void PrintConclusion(List<RealtimeTask> tasks, SystemParameters parameters)
    {
        SystemClass systemClass = TaskAnalysis.ClassifySystem(tasks);
        Architecture architecture = TaskAnalysis.ClassifyArchitecture(parameters.CpuCount, parameters.HasNetwork);
        RealtimeTask? critical = TaskAnalysis.CriticalTask(tasks);
        double reaction = TaskAnalysis.RequiredReactionTime(tasks);
        double utilization = TaskAnalysis.Utilization(tasks);
        bool loadAcceptable = TaskAnalysis.IsUtilizationAcceptable(utilization, parameters.CpuCount);
        List<RealtimeTask> infeasible = tasks.Where(task => !TaskAnalysis.IsFeasible(task)).ToList();

        Console.WriteLine($"Класс системы: {Localization.Name(systemClass)}.");
        Console.WriteLine($"Архитектура: {Localization.Name(architecture)}.");

        if (critical is null)
        {
            Console.WriteLine("Критическая задача: отсутствует (в наборе нет жёстких задач).");
        }
        else
        {
            Console.WriteLine($"Критическая задача: «{critical.Name}» (L = {Num(TaskAnalysis.Slack(critical))} мс).");
        }

        string scope = systemClass == SystemClass.HardRealTime ? "по жёстким задачам" : "по всем задачам";
        Console.WriteLine($"Требуемое время реакции R_треб = {Num(reaction)} мс ({scope}).");
        Console.WriteLine($"Коэффициент загрузки U = {utilization.ToString("0.00", Ru)} ({(utilization * PercentFactor).ToString("0.0", Ru)} %).");

        if (loadAcceptable)
        {
            Console.WriteLine($"Необходимое условие U <= {parameters.CpuCount} выполнено.");
        }
        else
        {
            Console.WriteLine($"Необходимое условие U <= {parameters.CpuCount} не выполнено: система перегружена.");
        }

        if (infeasible.Count == 0)
        {
            Console.WriteLine("Дедлайны всех задач выполнимы по собственному времени (L >= 0).");
        }
        else
        {
            string names = string.Join(", ", infeasible.Select(task => $"«{task.Name}»"));
            Console.WriteLine($"Невыполнимые дедлайны (L < 0): {names}.");
        }

        Console.WriteLine();
        if (infeasible.Count == 0 && loadAcceptable)
        {
            Console.WriteLine("Вывод: предварительные проверки пройдены; гарантия соблюдения дедлайнов требует анализа расписания.");
        }
        else
        {
            Console.WriteLine("Вывод: предварительные проверки не пройдены; набор задач невыполним без изменения параметров.");
        }
    }

    private static string Num(double value)
    {
        return value.ToString("G", Ru);
    }
}
