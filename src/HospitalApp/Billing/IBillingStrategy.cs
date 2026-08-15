// PHASE 2

namespace HospitalApp.Billing;

/// <summary>
/// Strategy in the Strategy pattern: an interchangeable billing algorithm,
/// selectable at runtime without modifying client code.
/// </summary>
public interface IBillingStrategy
{
    /// <summary>Human-readable name shown in menus and output.</summary>
    string Name { get; }

    /// <summary>Returns the amount the patient pays for the given base charge.</summary>
    decimal CalculateBill(decimal baseCharge);
}
