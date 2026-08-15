// PHASE 2

namespace HospitalApp.Billing;

/// <summary>
/// Billing for uninsured patients: the patient pays the full base charge.
/// </summary>
public class UninsuredBilling : IBillingStrategy
{
    public string Name => "Uninsured (patient pays full charge)";

    public decimal CalculateBill(decimal baseCharge)
    {
        if (baseCharge < 0)
            throw new ArgumentOutOfRangeException(nameof(baseCharge), "Base charge cannot be negative.");
        return Math.Round(baseCharge, 2);
    }
}
