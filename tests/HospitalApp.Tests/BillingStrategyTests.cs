// PHASE 2

using HospitalApp.Billing;
using HospitalApp.Services;

namespace HospitalApp.Tests;

public class BillingStrategyTests
{
    public BillingStrategyTests() => HospitalSystem.ResetInstance();

    [Fact]
    public void InsuredBilling_PatientPaysTwentyPercent()
    {
        Assert.Equal(200m, new InsuredBilling().CalculateBill(1000m));
    }

    [Fact]
    public void UninsuredBilling_PatientPaysFullCharge()
    {
        Assert.Equal(1000m, new UninsuredBilling().CalculateBill(1000m));
    }

    [Fact]
    public void GovernmentSubsidyBilling_PatientPaysFortyPercent()
    {
        Assert.Equal(400m, new GovernmentSubsidyBilling().CalculateBill(1000m));
    }

    [Fact]
    public void CalculateBill_NegativeChargeThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new InsuredBilling().CalculateBill(-1m));
    }

    [Fact]
    public void SetBillingStrategy_SwapsAlgorithmAtRuntime()
    {
        HospitalSystem hospital = HospitalSystem.Instance;
        Assert.IsType<UninsuredBilling>(hospital.BillingStrategy);
        Assert.Equal(1000m, hospital.BillingStrategy.CalculateBill(1000m));

        hospital.SetBillingStrategy(new InsuredBilling());

        Assert.Equal(200m, hospital.BillingStrategy.CalculateBill(1000m));
    }
}
