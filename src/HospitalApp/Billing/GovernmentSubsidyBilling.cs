// PHASE 2

namespace HospitalApp.Billing;

/// <summary>
/// Billing with a government subsidy: the subsidy covers 60% of the base
/// charge, the patient pays the remaining 40%.
/// </summary>
public class GovernmentSubsidyBilling : IBillingStrategy
{
    public const decimal PatientShare = 0.40m;

    public string Name => "Government subsidy (patient pays 40%)";

    public decimal CalculateBill(decimal baseCharge)
    {
        if (baseCharge < 0)
            throw new ArgumentOutOfRangeException(nameof(baseCharge), "Base charge cannot be negative.");
        return Math.Round(baseCharge * PatientShare, 2);
    }
}
