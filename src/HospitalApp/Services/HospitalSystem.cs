using HospitalApp.DataStructures;
using HospitalApp.Models;

namespace HospitalApp.Services;

/// <summary>
/// Singleton facade that wires together the registry, triage queue and
/// appointment service. Phase 2 will extend this with Observer notifications
/// and billing strategies.
/// </summary>
public sealed class HospitalSystem
{
    private static HospitalSystem _instance = new();

    public static HospitalSystem Instance => _instance;

    public PatientRegistry Registry { get; }
    public TriageQueue Triage { get; }
    public AppointmentService Appointments { get; }

    private HospitalSystem()
    {
        Registry = new PatientRegistry();
        Triage = new TriageQueue();
        Appointments = new AppointmentService();
    }

    /// <summary>Registers a patient and places them in the triage queue.</summary>
    public Patient AdmitPatient(int id, string name, int age, TriageLevel level)
    {
        var patient = new Patient(id, name, age, level);
        Registry.Register(patient);
        Triage.Enqueue(patient);
        return patient;
    }

    /// <summary>
    /// Dequeues the highest-priority patient and appends a treatment note
    /// to their medical history.
    /// </summary>
    public Patient TreatNext()
    {
        Patient patient = Triage.Dequeue();
        patient.History.AddRecord($"{DateTime.Now:yyyy-MM-dd}: Treated after triage ({patient.TriageLevel}).");
        return patient;
    }

    /// <summary>Replaces the singleton with a fresh instance. Intended for demos and tests.</summary>
    public static void ResetInstance() => _instance = new HospitalSystem();
}
