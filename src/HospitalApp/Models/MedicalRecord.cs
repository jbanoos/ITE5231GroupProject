namespace HospitalApp.Models;

/// <summary>
/// One immutable entry in a patient's medical history: when the patient was
/// seen, what they were diagnosed with, and what was prescribed. Records are
/// stored as nodes of the <see cref="DataStructures.MedicalHistory"/> linked list.
/// </summary>
public class MedicalRecord
{
    public DateTime Date { get; }
    public string Diagnosis { get; }

    /// <summary>What was prescribed, or an empty string when nothing was.</summary>
    public string Prescription { get; }

    public MedicalRecord(DateTime date, string diagnosis, string? prescription = null)
    {
        if (string.IsNullOrWhiteSpace(diagnosis))
            throw new ArgumentException("Diagnosis must not be empty.", nameof(diagnosis));

        Date = date;
        Diagnosis = diagnosis.Trim();
        Prescription = prescription?.Trim() ?? string.Empty;
    }

    public override string ToString() =>
        Prescription.Length == 0
            ? $"{Date:yyyy-MM-dd}: {Diagnosis}"
            : $"{Date:yyyy-MM-dd}: {Diagnosis} (Rx: {Prescription})";
}
