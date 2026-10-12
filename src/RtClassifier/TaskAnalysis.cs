namespace RtClassifier;

public static class TaskAnalysis
{
    public static double Slack(RealtimeTask task)
    {
        return task.D - task.C;
    }

    public static bool IsFeasible(RealtimeTask task)
    {
        return Slack(task) >= 0;
    }

    public static DeadlineType GetDeadlineType(RealtimeTask task)
    {
        if (task.D == task.T)
        {
            return DeadlineType.Implicit;
        }
        return task.D < task.T ? DeadlineType.Constrained : DeadlineType.Arbitrary;
    }

    public static SystemClass ClassifySystem(IEnumerable<RealtimeTask> tasks)
    {
        return tasks.Any(task => task.Hardness == Hardness.Hard)
            ? SystemClass.HardRealTime
            : SystemClass.SoftRealTime;
    }

    public static RealtimeTask? CriticalTask(IEnumerable<RealtimeTask> tasks)
    {
        RealtimeTask? critical = null;
        foreach (RealtimeTask task in tasks)
        {
            if (task.Hardness != Hardness.Hard)
            {
                continue;
            }
            if (critical is null || Slack(task) < Slack(critical))
            {
                critical = task;
            }
        }
        return critical;
    }

    public static double RequiredReactionTime(IEnumerable<RealtimeTask> tasks)
    {
        List<RealtimeTask> all = tasks.ToList();
        if (all.Count == 0)
        {
            throw new InvalidOperationException("Набор задач пуст, время реакции не определено.");
        }
        List<RealtimeTask> hard = all.Where(task => task.Hardness == Hardness.Hard).ToList();
        List<RealtimeTask> source = hard.Count > 0 ? hard : all;
        return source.Min(task => task.D);
    }

    public static double Utilization(IEnumerable<RealtimeTask> tasks)
    {
        return tasks.Sum(task => task.C / task.T);
    }

    public static bool IsUtilizationAcceptable(double utilization, int cpuCount)
    {
        return utilization <= cpuCount;
    }

    public static Architecture ClassifyArchitecture(int cpuCount, bool hasNetwork)
    {
        if (hasNetwork)
        {
            return Architecture.Distributed;
        }
        return cpuCount > 1 ? Architecture.Multiprocessor : Architecture.Uniprocessor;
    }
}
