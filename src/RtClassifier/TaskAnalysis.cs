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
}
