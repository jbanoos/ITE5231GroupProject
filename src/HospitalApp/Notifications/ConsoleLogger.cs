// PHASE 2

namespace HospitalApp.Notifications;

/// <summary>
/// Observer that logs every patient status change to the console.
/// </summary>
public class ConsoleLogger : IPatientStatusObserver
{
    private readonly TextWriter _output;

    public ConsoleLogger(TextWriter? output = null) => _output = output ?? Console.Out;

    public void OnStatusChanged(StatusChange change)
    {
        ArgumentNullException.ThrowIfNull(change);
        _output.WriteLine($"[LOG] {change}");
    }
}
