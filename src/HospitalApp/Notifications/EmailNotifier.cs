// PHASE 2

namespace HospitalApp.Notifications;

/// <summary>
/// Observer stub simulating email dispatch. Every message is recorded in
/// <see cref="SentMessages"/> so tests can verify what would have been sent.
/// </summary>
public class EmailNotifier : IPatientStatusObserver
{
    private readonly TextWriter _output;
    private readonly List<string> _sent = new();

    public EmailNotifier(TextWriter? output = null) => _output = output ?? Console.Out;

    public IReadOnlyList<string> SentMessages => _sent;

    public void OnStatusChanged(StatusChange change)
    {
        ArgumentNullException.ThrowIfNull(change);
        string message = $"Email to patient #{change.PatientId}: status {change.OldStatus} -> {change.NewStatus}";
        _sent.Add(message);
        _output.WriteLine($"[EMAIL] {message}");
    }
}
