namespace RtClassifier;

public static class Localization
{
    private static readonly Dictionary<string, Hardness> HardnessByText =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["жёсткое"] = Hardness.Hard,
            ["жесткое"] = Hardness.Hard,
            ["твёрдое"] = Hardness.Firm,
            ["твердое"] = Hardness.Firm,
            ["мягкое"] = Hardness.Soft
        };

    public static bool TryParseHardness(string? text, out Hardness hardness)
    {
        hardness = default;
        return text is not null && HardnessByText.TryGetValue(text.Trim(), out hardness);
    }

    public static string Name(Hardness hardness) => hardness switch
    {
        Hardness.Hard => "жёсткое",
        Hardness.Firm => "твёрдое",
        Hardness.Soft => "мягкое",
        _ => throw new ArgumentOutOfRangeException(nameof(hardness))
    };

    public static string Name(DeadlineType type) => type switch
    {
        DeadlineType.Implicit => "неявный",
        DeadlineType.Constrained => "ограниченный",
        DeadlineType.Arbitrary => "произвольный",
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };

    public static string Name(SystemClass systemClass) => systemClass switch
    {
        SystemClass.HardRealTime => "жёсткого реального времени",
        SystemClass.SoftRealTime => "мягкого реального времени",
        _ => throw new ArgumentOutOfRangeException(nameof(systemClass))
    };

    public static string Name(Architecture architecture) => architecture switch
    {
        Architecture.Uniprocessor => "однопроцессорная",
        Architecture.Multiprocessor => "многопроцессорная",
        Architecture.Distributed => "распределённая",
        _ => throw new ArgumentOutOfRangeException(nameof(architecture))
    };

    public static string YesNo(bool value) => value ? "Да" : "Нет";

    public static string Presence(bool value) => value ? "есть" : "нет";
}
