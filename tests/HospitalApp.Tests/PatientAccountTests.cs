// PHASE 2

using HospitalApp.Models;

namespace HospitalApp.Tests;

public class PatientAccountTests
{
    private static Patient NewPatient() => new(101, "Ava Smith", 34, TriageLevel.Standard);

    [Fact]
    public void NewAccount_StartsAtZero()
    {
        PatientAccount account = NewPatient().Account;

        Assert.Equal(0m, account.Balance);
        Assert.Equal(0m, account.TotalCharged);
        Assert.Equal(0m, account.TotalPaid);
        Assert.Empty(account.Payments);
    }

    [Fact]
    public void Charge_IncreasesBalanceAndTotalCharged()
    {
        PatientAccount account = NewPatient().Account;

        account.Charge(200m);
        account.Charge(50m);

        Assert.Equal(250m, account.Balance);
        Assert.Equal(250m, account.TotalCharged);
    }

    [Fact]
    public void Pay_PartialPayment_LeavesTheRemainderOutstanding()
    {
        PatientAccount account = NewPatient().Account;
        account.Charge(200m);

        Payment receipt = account.Pay(75m, PaymentMethod.Card);

        Assert.Equal(125m, account.Balance);
        Assert.Equal(75m, account.TotalPaid);
        Assert.Equal(200m, receipt.BalanceBefore);
        Assert.Equal(125m, receipt.BalanceAfter);
        Assert.Equal(PaymentMethod.Card, receipt.Method);
    }

    [Fact]
    public void Pay_FullBalance_ClearsTheAccount()
    {
        PatientAccount account = NewPatient().Account;
        account.Charge(200m);

        account.Pay(200m, PaymentMethod.Cash);

        Assert.Equal(0m, account.Balance);
    }

    [Fact]
    public void Pay_MoreThanTheBalance_ThrowsAndLeavesBalanceUnchanged()
    {
        PatientAccount account = NewPatient().Account;
        account.Charge(100m);

        Assert.Throws<InvalidOperationException>(() => account.Pay(150m, PaymentMethod.Cash));
        Assert.Equal(100m, account.Balance);
        Assert.Empty(account.Payments);
    }

    [Fact]
    public void Pay_ZeroOrNegative_ThrowsArgumentOutOfRangeException()
    {
        PatientAccount account = NewPatient().Account;
        account.Charge(100m);

        Assert.Throws<ArgumentOutOfRangeException>(() => account.Pay(0m, PaymentMethod.Cash));
        Assert.Throws<ArgumentOutOfRangeException>(() => account.Pay(-5m, PaymentMethod.Cash));
    }

    [Fact]
    public void Charge_ZeroOrNegative_ThrowsArgumentOutOfRangeException()
    {
        PatientAccount account = NewPatient().Account;

        Assert.Throws<ArgumentOutOfRangeException>(() => account.Charge(0m));
        Assert.Throws<ArgumentOutOfRangeException>(() => account.Charge(-1m));
    }

    [Fact]
    public void ReceiptNumbers_AreSequentialPerPatient()
    {
        PatientAccount account = NewPatient().Account;
        account.Charge(300m);

        Payment first = account.Pay(100m, PaymentMethod.Cash);
        Payment second = account.Pay(100m, PaymentMethod.Card);

        Assert.Equal("RCP-0101-001", first.ReceiptNumber);
        Assert.Equal("RCP-0101-002", second.ReceiptNumber);
        Assert.Equal(2, account.Payments.Count);
    }

    [Fact]
    public void ToReceipt_ShowsAmountAndBothBalances()
    {
        PatientAccount account = NewPatient().Account;
        account.Charge(200m);

        string receipt = account.Pay(50m, PaymentMethod.BankTransfer).ToReceipt();

        Assert.Contains("PAYMENT RECEIPT", receipt);
        Assert.Contains("RCP-0101-001", receipt);
        Assert.Contains("#101 Ava Smith", receipt);
        Assert.Contains("BankTransfer", receipt);
        Assert.Contains("200.00", receipt); // balance before
        Assert.Contains("50.00", receipt);  // amount paid
        Assert.Contains("150.00", receipt); // balance due
    }

    [Fact]
    public void ToStatement_ReportsTotalsAndListsReceipts()
    {
        PatientAccount account = NewPatient().Account;
        account.Charge(400m);
        account.Pay(150m, PaymentMethod.Card);

        string statement = account.ToStatement();

        Assert.Contains("Account statement for Ava Smith (#101)", statement);
        Assert.Contains("400.00", statement); // total charged
        Assert.Contains("150.00", statement); // total paid
        Assert.Contains("250.00", statement); // balance due
        Assert.Contains("RCP-0101-001", statement);
    }

    [Fact]
    public void ToStatement_WithNoPayments_SaysSo()
    {
        PatientAccount account = NewPatient().Account;
        account.Charge(100m);

        Assert.Contains("No payments recorded yet.", account.ToStatement());
    }
}
