namespace HospitalApp.Models;

/// <summary>
/// A scheduled appointment. Appointments are stored per doctor in a stack,
/// so the most recently scheduled appointment is cancelled first.
/// </summary>
public class Appointment
{
    public string DoctorName { get; }
    public int PatientId { get; }
    public DateTime ScheduledAt { get; }
    public string Reason { get; }

    public Appointment(string doctorName, int patientId, DateTime scheduledAt, string reason)
    {
        if (string.IsNullOrWhiteSpace(doctorName))
            throw new ArgumentException("Doctor name must not be empty.", nameof(doctorName));
        if (patientId <= 0)
            throw new ArgumentOutOfRangeException(nameof(patientId), "Patient id must be positive.");

        DoctorName = doctorName;
        PatientId = patientId;
        ScheduledAt = scheduledAt;
        Reason = reason ?? string.Empty;
    }

    public override string ToString() =>
        $"Dr. {DoctorName} with patient #{PatientId} at {ScheduledAt:yyyy-MM-dd HH:mm} ({Reason})";
}
