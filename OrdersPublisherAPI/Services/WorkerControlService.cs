namespace Services;

public class WorkerControlService
{
    public bool IsEnabled { get; set; } =  true;
    public TimeSpan ExecutionTimeout { get; set; } = new TimeSpan(0, 0, 0);
}