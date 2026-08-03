// PHASE 1

using HospitalApp.Models;

namespace HospitalApp.Services;

/// <summary>
/// Central patient registry backed by a <see cref="Dictionary{TKey,TValue}"/>
/// keyed on patient id for O(1) lookups.
/// </summary>
/// <remarks>
/// Time complexity: Register, GetById, TryGetById, Update and Remove are all
/// O(1) on average — hashing the integer id goes straight to a bucket. They
/// degrade to O(n) only in the pathological case where every key collides,
/// which integer ids do not. Count is O(1); GetAll is O(n) because it copies
/// the values into a new list.
/// </remarks>
public class PatientRegistry
{
    private readonly Dictionary<int, Patient> _patients = new();

    public int Count => _patients.Count;

    /// <summary>Adds a patient. Throws if the id is already registered. O(1) amortised.</summary>
    public void Register(Patient patient)
    {
        ArgumentNullException.ThrowIfNull(patient);
        if (!_patients.TryAdd(patient.Id, patient))
            throw new InvalidOperationException($"A patient with ID {patient.Id} is already registered.");
    }

    /// <summary>Returns the patient with the given id, or throws <see cref="KeyNotFoundException"/>. O(1).</summary>
    public Patient GetById(int id) =>
        _patients.TryGetValue(id, out Patient? patient)
            ? patient
            : throw new KeyNotFoundException($"No patient registered with ID {id}.");

    public bool TryGetById(int id, out Patient? patient) => _patients.TryGetValue(id, out patient);

    /// <summary>Replaces an existing patient record. Returns false if the id is not registered. O(1).</summary>
    public bool Update(Patient patient)
    {
        ArgumentNullException.ThrowIfNull(patient);
        if (!_patients.ContainsKey(patient.Id))
            return false;

        _patients[patient.Id] = patient;
        return true;
    }

    /// <summary>Removes the patient with the given id. Returns false if not found. O(1).</summary>
    public bool Remove(int id) => _patients.Remove(id);

    public IReadOnlyCollection<Patient> GetAll() => _patients.Values.ToList();
}
