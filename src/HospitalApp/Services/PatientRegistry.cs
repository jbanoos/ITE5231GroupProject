using HospitalApp.Models;

namespace HospitalApp.Services;

/// <summary>
/// Central patient registry backed by a <see cref="Dictionary{TKey,TValue}"/>
/// keyed on patient id for O(1) lookups.
/// </summary>
public class PatientRegistry
{
    private readonly Dictionary<int, Patient> _patients = new();

    public int Count => _patients.Count;

    /// <summary>Adds a patient. Throws if the id is already registered.</summary>
    public void Register(Patient patient)
    {
        ArgumentNullException.ThrowIfNull(patient);
        if (!_patients.TryAdd(patient.Id, patient))
            throw new InvalidOperationException($"A patient with ID {patient.Id} is already registered.");
    }

    /// <summary>Returns the patient with the given id, or throws <see cref="KeyNotFoundException"/>.</summary>
    public Patient GetById(int id) =>
        _patients.TryGetValue(id, out Patient? patient)
            ? patient
            : throw new KeyNotFoundException($"No patient registered with ID {id}.");

    public bool TryGetById(int id, out Patient? patient) => _patients.TryGetValue(id, out patient);

    /// <summary>Replaces an existing patient record. Returns false if the id is not registered.</summary>
    public bool Update(Patient patient)
    {
        ArgumentNullException.ThrowIfNull(patient);
        if (!_patients.ContainsKey(patient.Id))
            return false;

        _patients[patient.Id] = patient;
        return true;
    }

    /// <summary>Removes the patient with the given id. Returns false if not found.</summary>
    public bool Remove(int id) => _patients.Remove(id);

    public IReadOnlyCollection<Patient> GetAll() => _patients.Values.ToList();
}
