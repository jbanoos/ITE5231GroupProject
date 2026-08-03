// PHASE 1

using HospitalApp.DataStructures;
using HospitalApp.Models;
// PHASE 2
using HospitalApp.Billing;
using HospitalApp.Notifications;
// END OF PHASE 2 BLOCK

namespace HospitalApp.Services;

/// <summary>
/// Singleton facade that wires together the registry, triage queue and
/// appointment service. Phase 2 added Observer notifications (status changes
/// broadcast through the <see cref="PatientStatusNotifier"/>) and a
/// runtime-swappable <see cref="IBillingStrategy"/>.
/// </summary>
public sealed class HospitalSystem
{
    private static HospitalSystem _instance = new();

    public static HospitalSystem Instance => _instance;

    public PatientRegistry Registry { get; }
    public TriageQueue Triage { get; }
    public AppointmentService Appointments { get; }

    // PHASE 2
    /// <summary>Subject that broadcasts patient status changes to all observers.</summary>
    public PatientStatusNotifier Notifier { get; }

    /// <summary>The billing strategy currently in effect; swappable at runtime.</summary>
    public IBillingStrategy BillingStrategy { get; private set; }
    // END OF PHASE 2 BLOCK

    private HospitalSystem()
    {
        Registry = new PatientRegistry();
        Triage = new TriageQueue();
        Appointments = new AppointmentService();
        // PHASE 2
        Notifier = new PatientStatusNotifier();
        BillingStrategy = new UninsuredBilling();
        // END OF PHASE 2 BLOCK
    }

    /// <summary>Registers a patient and places them in the triage queue.</summary>
    public Patient AdmitPatient(int id, string name, int age, TriageLevel level)
    {
        var patient = new Patient(id, name, age, level);
        Registry.Register(patient);
        Triage.Enqueue(patient);
        // PHASE 2
        ChangeStatus(patient, PatientStatus.Waiting);
        // END OF PHASE 2 BLOCK
        return patient;
    }

    /// <summary>
    /// Dequeues the highest-priority patient and appends a treatment note
    /// to their medical history.
    /// </summary>
    public Patient TreatNext()
    {
        Patient patient = Triage.Dequeue();
        // PHASE 2
        ChangeStatus(patient, PatientStatus.InExamination);
        // END OF PHASE 2 BLOCK
        patient.History.AddRecord(new MedicalRecord(
            DateTime.Now,
            $"Treated after triage ({patient.TriageLevel})"));
        // PHASE 2
        ChangeStatus(patient, PatientStatus.Treated);
        // END OF PHASE 2 BLOCK
        return patient;
    }

    // PHASE 2
    /// <summary>Updates the patient's status and broadcasts the change to all observers.</summary>
    public void ChangeStatus(Patient patient, PatientStatus newStatus)
    {
        ArgumentNullException.ThrowIfNull(patient);
        var change = new StatusChange(patient.Id, patient.Name, patient.Status, newStatus);
        patient.Status = newStatus;
        Notifier.Notify(change);
    }

    /// <summary>Selects the billing strategy used for future bills at runtime.</summary>
    public void SetBillingStrategy(IBillingStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(strategy);
        BillingStrategy = strategy;
    }
    // END OF PHASE 2 BLOCK

    // PHASE 2
    /// <summary>
    /// Prices the base charge with the current billing strategy, adds it to the
    /// patient's outstanding balance and notes it in their history.
    /// Returns the amount charged.
    /// </summary>
    public decimal BillPatient(Patient patient, decimal baseCharge)
    {
        ArgumentNullException.ThrowIfNull(patient);

        decimal amount = BillingStrategy.CalculateBill(baseCharge);
        if (amount > 0)
            patient.Account.Charge(amount);

        patient.History.AddRecord(new MedicalRecord(
            DateTime.Now,
            $"Billed {amount:0.00} ({BillingStrategy.Name})"));

        return amount;
    }

    /// <summary>
    /// Settles part or all of the patient's balance and returns the receipt.
    /// The payment is also noted in the patient's medical history.
    /// </summary>
    public Payment RecordPayment(Patient patient, decimal amount, PaymentMethod method)
    {
        ArgumentNullException.ThrowIfNull(patient);

        Payment receipt = patient.Account.Pay(amount, method);

        patient.History.AddRecord(new MedicalRecord(
            DateTime.Now,
            $"Payment {receipt.Amount:0.00} received ({method}), receipt {receipt.ReceiptNumber}"));

        return receipt;
    }

    /// <summary>Total amount owed across every registered patient. O(n).</summary>
    public decimal TotalOutstanding() => Registry.GetAll().Sum(p => p.Account.Balance);
    // END OF PHASE 2 BLOCK

    /// <summary>Replaces the singleton with a fresh instance. Intended for demos and tests.</summary>
    public static void ResetInstance() => _instance = new HospitalSystem();
}
