// PHASE 2

namespace HospitalApp.Models;

/// <summary>
/// An immutable record of one payment against a patient's account, and the
/// receipt issued for it. Created by <see cref="PatientAccount.Pay"/>, which is
/// what assigns the receipt number and the before/after balances.
/// </summary>
public class Payment
{
    public string ReceiptNumber { get; }
    public int PatientId { get; }
    public string PatientName { get; }
    public decimal Amount { get; }
    public PaymentMethod Method { get; }

    /// <summary>Outstanding balance immediately before this payment.</summary>
    public decimal BalanceBefore { get; }

    /// <summary>Outstanding balance left after this payment.</summary>
    public decimal BalanceAfter { get; }

    public DateTime PaidAt { get; }

    internal Payment(
        string receiptNumber,
        int patientId,
        string patientName,
        decimal amount,
        PaymentMethod method,
        decimal balanceBefore,
        decimal balanceAfter)
    {
        ReceiptNumber = receiptNumber;
        PatientId = patientId;
        PatientName = patientName;
        Amount = amount;
        Method = method;
        BalanceBefore = balanceBefore;
        BalanceAfter = balanceAfter;
        PaidAt = DateTime.Now;
    }

    /// <summary>Renders the payment as a printable receipt block.</summary>
    public string ToReceipt() => string.Join(Environment.NewLine,
        "========================================",
        "            PAYMENT RECEIPT",
        "========================================",
        $"Receipt no.    : {ReceiptNumber}",
        $"Date           : {PaidAt:yyyy-MM-dd HH:mm}",
        $"Patient        : #{PatientId} {PatientName}",
        $"Method         : {Method}",
        "----------------------------------------",
        $"Balance before : {BalanceBefore,12:0.00}",
        $"Amount paid    : {Amount,12:0.00}",
        $"Balance due    : {BalanceAfter,12:0.00}",
        "========================================");

    public override string ToString() =>
        $"{ReceiptNumber}  {PaidAt:yyyy-MM-dd HH:mm}  {Method,-12} {Amount,10:0.00}  -> balance {BalanceAfter:0.00}";
}
