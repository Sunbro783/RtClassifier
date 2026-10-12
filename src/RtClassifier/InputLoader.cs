using System.Text.Json;

namespace RtClassifier;

internal sealed record RawTask(string? Name, string? Hardness, double C, double D, double T);

internal sealed record RawParameters(int CpuCount, bool HasNetwork);

internal sealed record RawInput(string? SystemName, RawParameters? Parameters, List<RawTask>? Tasks);

public static class InputLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static InputData Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Файл не найден: {path}");
        }

        string json = File.ReadAllText(path);
        RawInput raw = JsonSerializer.Deserialize<RawInput>(json, Options)
            ?? throw new InvalidDataException("Файл пуст или имеет неверный формат.");

        return Convert(raw);
    }

    private static InputData Convert(RawInput raw)
    {
        if (string.IsNullOrWhiteSpace(raw.SystemName))
        {
            throw new InvalidDataException("Не указано название системы (systemName).");
        }
        if (raw.Parameters is null)
        {
            throw new InvalidDataException("Не заданы параметры системы (parameters).");
        }
        if (raw.Parameters.CpuCount < 1)
        {
            throw new InvalidDataException("Число процессоров должно быть не меньше 1.");
        }
        if (raw.Tasks is null || raw.Tasks.Count == 0)
        {
            throw new InvalidDataException("В файле нет задач.");
        }

        List<RealtimeTask> tasks = new();
        foreach (RawTask item in raw.Tasks)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                throw new InvalidDataException("У одной из задач не указано имя.");
            }
            if (!Localization.TryParseHardness(item.Hardness, out Hardness hardness))
            {
                throw new InvalidDataException($"Задача «{item.Name}»: неизвестный класс жёсткости.");
            }
            if (item.C <= 0 || item.D <= 0 || item.T <= 0)
            {
                throw new InvalidDataException($"Задача «{item.Name}»: значения C, D и T должны быть положительными.");
            }
            tasks.Add(new RealtimeTask(item.Name.Trim(), hardness, item.C, item.D, item.T));
        }

        SystemParameters parameters = new(raw.Parameters.CpuCount, raw.Parameters.HasNetwork);
        return new InputData(raw.SystemName.Trim(), parameters, tasks);
    }
}
