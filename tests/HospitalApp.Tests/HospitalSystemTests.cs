using HospitalApp.Models;
using HospitalApp.Services;

namespace HospitalApp.Tests;

public class HospitalSystemTests
{
    public HospitalSystemTests() => HospitalSystem.ResetInstance();

    [Fact]
    public void Instance_AlwaysReturnsTheSameSingleton()
    {
        Assert.Same(HospitalSystem.Instance, HospitalSystem.Instance);
    }

    [Fact]
    public void AdmitPatient_RegistersInRegistryAndEnqueuesInTriage()
    {
        HospitalSystem hospital = HospitalSystem.Instance;

        Patient patient = hospital.AdmitPatient(1, "Ava Smith", 34, TriageLevel.Urgent);

        Assert.Same(patient, hospital.Registry.GetById(1));
        Assert.Same(patient, hospital.Triage.Peek());
    }

    [Fact]
    public void TreatNext_TakesHighestPriorityAndAppendsHistoryRecord()
    {
        HospitalSystem hospital = HospitalSystem.Instance;
        hospital.AdmitPatient(1, "Ava Smith", 34, TriageLevel.Standard);
        hospital.AdmitPatient(2, "Ben Cole", 58, TriageLevel.Critical);

        Patient treated = hospital.TreatNext();

        Assert.Equal(2, treated.Id);
        Assert.Equal(1, treated.History.Count);
        Assert.Equal(1, hospital.Triage.Count);
    }
}
