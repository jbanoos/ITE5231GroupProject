// PHASE 2

namespace HospitalApp.Billing;

/// <summary>
/// Billing for insured patients: insurance covers 80% of the base charge,
/// the patient pays the remaining 20%.
/// </summary>
public class InsuredBilling : IBillingStrategy
{
    public const decimal PatientShare = 0.20m;

    public string Name => "Insured (patient pays 20%)";

    public decimal CalculateBill(decimal baseCharge)
    {
        if (baseCharge < 0)
            throw new ArgumentOutOfRangeException(nameof(baseCharge), "Base charge cannot be negative.");
        return Math.Round(baseCharge * PatientShare, 2);
    }
}
