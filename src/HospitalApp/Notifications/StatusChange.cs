// PHASE 2

using HospitalApp.Models;

namespace HospitalApp.Notifications;

/// <summary>
/// Immutable record of one patient status transition. Instances are staged in a
/// FIFO queue by the PatientStatusNotifier and then dispatched to observers.
/// </summary>
public class StatusChange
{
    public int PatientId { get; }
    public string PatientName { get; }
    public PatientStatus OldStatus { get; }
    public PatientStatus NewStatus { get; }
    public DateTime OccurredAt { get; }

    public StatusChange(int patientId, string patientName, PatientStatus oldStatus, PatientStatus newStatus)
    {
        if (patientId <= 0)
            throw new ArgumentOutOfRangeException(nameof(patientId), "Patient id must be positive.");
        if (string.IsNullOrWhiteSpace(patientName))
            throw new ArgumentException("Patient name must not be empty.", nameof(patientName));

        PatientId = patientId;
        PatientName = patientName;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        OccurredAt = DateTime.Now;
    }

    public override string ToString() =>
        $"[{OccurredAt:HH:mm:ss}] Patient #{PatientId} {PatientName}: {OldStatus} -> {NewStatus}";
}
