using HospitalApp.DataStructures;

namespace HospitalApp.Models;

/// <summary>
/// A registered hospital patient. Each patient owns a linked-list medical history.
/// </summary>
public class Patient
{
    public int Id { get; }
    public string Name { get; set; }
    public int Age { get; set; }
    public TriageLevel TriageLevel { get; set; }

    // PHASE 2
    /// <summary>Current lifecycle status; changes are broadcast to all registered observers.</summary>
    public PatientStatus Status { get; set; } = PatientStatus.Registered;
    // END OF PHASE 2 BLOCK

    public MedicalHistory History { get; }

    public Patient(int id, string name, int age, TriageLevel triageLevel)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Patient id must be positive.");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Patient name must not be empty.", nameof(name));
        if (age < 0)
            throw new ArgumentOutOfRangeException(nameof(age), "Age cannot be negative.");

        Id = id;
        Name = name;
        Age = age;
        TriageLevel = triageLevel;
        History = new MedicalHistory();
    }

    public override string ToString() => $"#{Id} {Name} (age {Age}) - {TriageLevel}";
}
