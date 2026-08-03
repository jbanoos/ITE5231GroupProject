// PHASE 1

using HospitalApp.Models;
using HospitalApp.Services;

namespace HospitalApp.Tests;

public class PatientRegistryTests
{
    [Fact]
    public void Register_ThenGetById_ReturnsSamePatient()
    {
        var registry = new PatientRegistry();
        var patient = new Patient(1, "Ava Smith", 34, TriageLevel.Standard);

        registry.Register(patient);

        Assert.Equal(1, registry.Count);
        Assert.Same(patient, registry.GetById(1));
    }

    [Fact]
    public void Register_DuplicateId_ThrowsInvalidOperationException()
    {
        var registry = new PatientRegistry();
        registry.Register(new Patient(1, "Ava Smith", 34, TriageLevel.Standard));

        Assert.Throws<InvalidOperationException>(
            () => registry.Register(new Patient(1, "Ben Cole", 58, TriageLevel.Critical)));
    }

    [Fact]
    public void GetById_UnknownId_ThrowsKeyNotFoundException()
    {
        var registry = new PatientRegistry();

        Assert.Throws<KeyNotFoundException>(() => registry.GetById(999));
    }

    [Fact]
    public void Update_ExistingPatient_ReplacesStoredRecord()
    {
        var registry = new PatientRegistry();
        registry.Register(new Patient(1, "Ava Smith", 34, TriageLevel.Standard));
        var updated = new Patient(1, "Ava Smith-Jones", 35, TriageLevel.Urgent);

        Assert.True(registry.Update(updated));

        Assert.Same(updated, registry.GetById(1));
        Assert.Equal("Ava Smith-Jones", registry.GetById(1).Name);
    }

    [Fact]
    public void Update_UnknownId_ReturnsFalse()
    {
        var registry = new PatientRegistry();

        Assert.False(registry.Update(new Patient(99, "Nobody", 20, TriageLevel.Standard)));
        Assert.Equal(0, registry.Count);
    }

    [Fact]
    public void Remove_ExistingId_ReturnsTrueAndRemovesPatient()
    {
        var registry = new PatientRegistry();
        registry.Register(new Patient(1, "Ava Smith", 34, TriageLevel.Standard));

        Assert.True(registry.Remove(1));
        Assert.Equal(0, registry.Count);
        Assert.False(registry.TryGetById(1, out _));
    }
}
