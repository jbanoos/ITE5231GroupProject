// PHASE 2

namespace HospitalApp.Notifications;

/// <summary>
/// Observer stub simulating SMS dispatch. Every message is recorded in
/// <see cref="SentMessages"/> so tests can verify what would have been sent.
/// </summary>
public class SmsNotifier : IPatientStatusObserver
{
    private readonly TextWriter _output;
    private readonly List<string> _sent = new();

    public SmsNotifier(TextWriter? output = null) => _output = output ?? Console.Out;

    public IReadOnlyList<string> SentMessages => _sent;

    public void OnStatusChanged(StatusChange change)
    {
        ArgumentNullException.ThrowIfNull(change);
        string message = $"SMS to patient #{change.PatientId}: status {change.OldStatus} -> {change.NewStatus}";
        _sent.Add(message);
        _output.WriteLine($"[SMS] {message}");
    }
}
