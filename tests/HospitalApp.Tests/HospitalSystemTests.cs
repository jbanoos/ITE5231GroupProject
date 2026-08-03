// PHASE 1

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

    // PHASE 2
    [Fact]
    public void BillPatient_AppliesTheStrategyAndChargesTheAccount()
    {
        HospitalSystem hospital = HospitalSystem.Instance;
        Patient patient = hospital.AdmitPatient(1, "Ava Smith", 34, TriageLevel.Standard);
        hospital.SetBillingStrategy(new HospitalApp.Billing.InsuredBilling());

        decimal amount = hospital.BillPatient(patient, 1000m);

        Assert.Equal(200m, amount);
        Assert.Equal(200m, patient.Account.Balance);
    }

    [Fact]
    public void RecordPayment_ReducesTheBalanceAndReturnsAReceipt()
    {
        HospitalSystem hospital = HospitalSystem.Instance;
        Patient patient = hospital.AdmitPatient(1, "Ava Smith", 34, TriageLevel.Standard);
        hospital.BillPatient(patient, 500m);

        Payment receipt = hospital.RecordPayment(patient, 200m, PaymentMethod.Card);

        Assert.Equal(300m, patient.Account.Balance);
        Assert.Equal(200m, receipt.Amount);
        Assert.Equal("RCP-0001-001", receipt.ReceiptNumber);
    }

    [Fact]
    public void TotalOutstanding_SumsBalancesAcrossPatients()
    {
        HospitalSystem hospital = HospitalSystem.Instance;
        Patient ava = hospital.AdmitPatient(1, "Ava Smith", 34, TriageLevel.Standard);
        Patient ben = hospital.AdmitPatient(2, "Ben Cole", 58, TriageLevel.Urgent);

        hospital.BillPatient(ava, 300m);
        hospital.BillPatient(ben, 200m);
        hospital.RecordPayment(ava, 100m, PaymentMethod.Cash);

        Assert.Equal(400m, hospital.TotalOutstanding());
    }
    // END OF PHASE 2 BLOCK
}
