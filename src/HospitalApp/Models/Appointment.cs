// PHASE 1

namespace HospitalApp.Models;

/// <summary>
/// A scheduled appointment. Appointments are stored per doctor in a stack,
/// so the most recently scheduled appointment is cancelled first.
/// </summary>
/// <remarks>
/// The scheduled date is validated in the constructor, so an invalid
/// appointment can never exist — every caller gets the same guarantee without
/// having to repeat the checks.
/// </remarks>
public class Appointment
{
    // PHASE 2
    /// <summary>How far ahead an appointment may be booked.</summary>
    public const int MaxAdvanceDays = 365;

    /// <summary>Earliest start time the clinic accepts.</summary>
    public static readonly TimeSpan OpensAt = new(8, 0, 0);

    /// <summary>Appointments must start before this time.</summary>
    public static readonly TimeSpan ClosesAt = new(18, 0, 0);
    // END OF PHASE 2 BLOCK

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

        // PHASE 2
        ValidateScheduledAt(scheduledAt);
        // END OF PHASE 2 BLOCK

        DoctorName = doctorName;
        PatientId = patientId;
        ScheduledAt = scheduledAt;
        Reason = reason ?? string.Empty;
    }

    // PHASE 2
    /// <summary>
    /// Rejects dates in the past, dates too far ahead, and times outside
    /// clinic hours.
    /// </summary>
    private static void ValidateScheduledAt(DateTime scheduledAt)
    {
        DateTime now = DateTime.Now;

        if (scheduledAt <= now)
            throw new ArgumentOutOfRangeException(nameof(scheduledAt),
                $"Appointment must be in the future; {scheduledAt:yyyy-MM-dd HH:mm} has already passed.");

        if (scheduledAt > now.AddDays(MaxAdvanceDays))
            throw new ArgumentOutOfRangeException(nameof(scheduledAt),
                $"Appointments cannot be booked more than {MaxAdvanceDays} days ahead.");

        if (scheduledAt.TimeOfDay < OpensAt || scheduledAt.TimeOfDay >= ClosesAt)
            throw new ArgumentOutOfRangeException(nameof(scheduledAt),
                $"Appointments must start between {OpensAt:hh\\:mm} and {ClosesAt:hh\\:mm}; " +
                $"{scheduledAt:HH:mm} is outside clinic hours.");
    }
    // END OF PHASE 2 BLOCK

    public override string ToString() =>
        $"Dr. {DoctorName} with patient #{PatientId} at {ScheduledAt:yyyy-MM-dd HH:mm} ({Reason})";
}
