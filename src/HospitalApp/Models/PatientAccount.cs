// PHASE 2

namespace HospitalApp.Models;

/// <summary>
/// A patient's billing account: the running outstanding balance plus every
/// receipt issued against it. Owned by <see cref="Patient"/>, so an account can
/// never drift away from the patient it belongs to.
/// </summary>
/// <remarks>
/// Time complexity: Charge and Pay are O(1) — the balance is a running total,
/// and a receipt is appended to the end of the payment list. Payments is O(1)
/// to access and O(n) to walk.
/// </remarks>
public class PatientAccount
{
    private readonly Patient _patient;
    private readonly List<Payment> _payments = new();

    internal PatientAccount(Patient patient) => _patient = patient;

    /// <summary>Amount still owed. Never negative: payments cannot exceed it.</summary>
    public decimal Balance { get; private set; }

    public decimal TotalCharged { get; private set; }
    public decimal TotalPaid { get; private set; }

    /// <summary>Receipts issued against this account, oldest first.</summary>
    public IReadOnlyList<Payment> Payments => _payments;

    /// <summary>Adds a charge to the outstanding balance.</summary>
    public void Charge(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "A charge must be greater than zero.");

        amount = decimal.Round(amount, 2);
        Balance += amount;
        TotalCharged += amount;
    }

    /// <summary>
    /// Settles part or all of the outstanding balance and returns the receipt.
    /// Overpayment is rejected so the balance can never go negative.
    /// </summary>
    public Payment Pay(decimal amount, PaymentMethod method)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "A payment must be greater than zero.");

        amount = decimal.Round(amount, 2);
        if (amount > Balance)
            throw new InvalidOperationException(
                $"Payment of {amount:0.00} exceeds the outstanding balance of {Balance:0.00}.");

        decimal balanceBefore = Balance;
        Balance -= amount;
        TotalPaid += amount;

        var payment = new Payment(
            $"RCP-{_patient.Id:D4}-{_payments.Count + 1:D3}",
            _patient.Id,
            _patient.Name,
            amount,
            method,
            balanceBefore,
            Balance);

        _payments.Add(payment);
        return payment;
    }

    /// <summary>Renders the account as a printable statement with every receipt listed.</summary>
    public string ToStatement()
    {
        var lines = new List<string>
        {
            $"Account statement for {_patient.Name} (#{_patient.Id})",
            $"  Total charged : {TotalCharged,12:0.00}",
            $"  Total paid    : {TotalPaid,12:0.00}",
            $"  Balance due   : {Balance,12:0.00}"
        };

        if (_payments.Count == 0)
        {
            lines.Add("  No payments recorded yet.");
        }
        else
        {
            lines.Add($"  Receipts ({_payments.Count}):");
            foreach (Payment payment in _payments)
                lines.Add("    " + payment);
        }

        return string.Join(Environment.NewLine, lines);
    }
}
