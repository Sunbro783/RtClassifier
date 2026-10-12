namespace RtClassifier;

public enum Hardness
{
    Hard,
    Firm,
    Soft
}

public enum DeadlineType
{
    Implicit,
    Constrained,
    Arbitrary
}

public enum SystemClass
{
    HardRealTime,
    SoftRealTime
}

public enum Architecture
{
    Uniprocessor,
    Multiprocessor,
    Distributed
}

public sealed record RealtimeTask(
    string Name,
    Hardness Hardness,
    double C,
    double D,
    double T);

public sealed record SystemParameters(
    int CpuCount,
    bool HasNetwork);

public sealed record InputData(
    string SystemName,
    SystemParameters Parameters,
    List<RealtimeTask> Tasks);
