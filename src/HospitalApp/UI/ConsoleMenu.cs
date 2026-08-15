// PHASE 2

using HospitalApp.Billing;
using HospitalApp.DataStructures;
using HospitalApp.Models;
using HospitalApp.Notifications;
using HospitalApp.Services;

namespace HospitalApp.UI;

/// <summary>
/// Menu-driven console workflow tying every subsystem together: admission,
/// triage, treatment, appointments, medical history and billing. The console
/// logger, SMS stub and email stub observers are registered at startup, so
/// every patient status change is broadcast immediately. Input/output go
/// through <see cref="TextReader"/>/<see cref="TextWriter"/> so the workflow
/// can be driven from tests.
/// </summary>
public class ConsoleMenu
{
    private readonly HospitalSystem _hospital;
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public ConsoleMenu(HospitalSystem hospital, TextReader input, TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(hospital);
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        _hospital = hospital;
        _input = input;
        _output = output;
    }

    /// <summary>Runs the menu loop until the user chooses Exit.</summary>
    public void Run()
    {
        _hospital.Notifier.Subscribe(new ConsoleLogger(_output));
        _hospital.Notifier.Subscribe(new SmsNotifier(_output));
        _hospital.Notifier.Subscribe(new EmailNotifier(_output));

        _output.WriteLine("=== Hospital Patient Management System ===");

        bool running = true;
        while (running)
        {
            PrintMenu();
            try
            {
                switch (ReadInt("Choose an option: "))
                {
                    case 1: AdmitPatient(); break;
                    case 2: ViewTriageQueue(); break;
                    case 3: TreatNextPatient(); break;
                    case 4: ScheduleAppointment(); break;
                    case 5: CancelAppointment(); break;
                    case 6: ViewAppointments(); break;
                    case 7: AddMedicalRecord(); break;
                    case 8: ViewMedicalHistory(); break;
                    case 9: ChooseBillingStrategy(); break;
                    case 10: CalculateBill(); break;
                    case 11: RecordPayment(); break;
                    case 12: ViewAccountStatement(); break;
                    case 0: running = false; break;
                    default: _output.WriteLine("Unknown option, try again."); break;
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException
                                       or ArgumentException or FormatException)
            {
                _output.WriteLine($"Error: {ex.Message}");
            }
            _output.WriteLine();
        }

        _output.WriteLine("Goodbye.");
    }

    private void PrintMenu()
    {
        _output.WriteLine();
        _output.WriteLine($"Billing strategy: {_hospital.BillingStrategy.Name} | " +
                          $"Patients: {_hospital.Registry.Count} | In triage: {_hospital.Triage.Count} | " +
                          $"Outstanding: {_hospital.TotalOutstanding():0.00}");
        _output.WriteLine(" 1. Admit patient");
        _output.WriteLine(" 2. View triage queue");
        _output.WriteLine(" 3. Treat next patient");
        _output.WriteLine(" 4. Schedule appointment");
        _output.WriteLine(" 5. Cancel most recent appointment");
        _output.WriteLine(" 6. View doctor's next appointment");
        _output.WriteLine(" 7. Add medical record");
        _output.WriteLine(" 8. View medical history");
        _output.WriteLine(" 9. Choose billing strategy");
        _output.WriteLine("10. Bill patient");
        _output.WriteLine("11. Record payment");
        _output.WriteLine("12. View account statement");
        _output.WriteLine(" 0. Exit");
    }

    private void AdmitPatient()
    {
        int? id = ReadInt("Patient id: ");
        if (id is null) { Invalid("a whole number"); return; }
        string name = ReadText("Name: ");
        int? age = ReadInt("Age: ");
        if (age is null) { Invalid("a whole number"); return; }
        int? level = ReadInt("Triage level (1=Critical, 2=Urgent, 3=Standard): ");
        if (level is null or < 1 or > 3) { Invalid("1, 2 or 3"); return; }

        Patient patient = _hospital.AdmitPatient(id.Value, name, age.Value, (TriageLevel)level.Value);
        _output.WriteLine($"Admitted: {patient}");
    }

    private void ViewTriageQueue()
    {
        IReadOnlyList<Patient> snapshot = _hospital.Triage.Snapshot();
        if (snapshot.Count == 0)
        {
            _output.WriteLine("Triage queue is empty.");
            return;
        }

        _output.WriteLine("Triage order (highest priority first):");
        foreach (Patient p in snapshot)
            _output.WriteLine($"  {p} [{p.Status}]");
    }

    private void TreatNextPatient()
    {
        Patient treated = _hospital.TreatNext();
        _output.WriteLine($"Treated: {treated}");
    }

    private void ScheduleAppointment()
    {
        string doctor = ReadText("Doctor name: ");
        int? patientId = ReadInt("Patient id: ");
        if (patientId is null) { Invalid("a whole number"); return; }
        string when = ReadText($"Date and time (yyyy-MM-dd HH:mm, " +
                               $"{Appointment.OpensAt:hh\\:mm}-{Appointment.ClosesAt:hh\\:mm}): ");
        if (!DateTime.TryParse(when, out DateTime scheduledAt)) { Invalid("a date and time"); return; }
        string reason = ReadText("Reason: ");

        _hospital.Registry.GetById(patientId.Value); // throws if the patient is unknown
        var appointment = new Appointment(doctor, patientId.Value, scheduledAt, reason);
        _hospital.Appointments.Schedule(appointment);
        _output.WriteLine($"Scheduled: {appointment}");
    }

    private void CancelAppointment()
    {
        string doctor = ReadText("Doctor name: ");
        Appointment cancelled = _hospital.Appointments.CancelMostRecent(doctor);
        _output.WriteLine($"Cancelled: {cancelled}");
    }

    private void ViewAppointments()
    {
        string doctor = ReadText("Doctor name: ");
        Appointment? next = _hospital.Appointments.PeekNext(doctor);
        _output.WriteLine(next is null
            ? $"Dr. {doctor} has no appointments."
            : $"Next for Dr. {doctor}: {next} (total {_hospital.Appointments.CountFor(doctor)})");
    }

    private void AddMedicalRecord()
    {
        int? patientId = ReadInt("Patient id: ");
        if (patientId is null) { Invalid("a whole number"); return; }

        string dateText = ReadText("Date (yyyy-MM-dd, blank for today): ");
        DateTime date;
        if (dateText.Length == 0)
            date = DateTime.Now;
        else if (!DateTime.TryParse(dateText, out date)) { Invalid("a date"); return; }

        string diagnosis = ReadText("Diagnosis: ");
        if (diagnosis.Length == 0) { Invalid("a diagnosis"); return; }
        string prescription = ReadText("Prescription (blank if none): ");

        Patient patient = _hospital.Registry.GetById(patientId.Value);
        patient.History.AddRecord(new MedicalRecord(date, diagnosis, prescription));
        _output.WriteLine($"Record added to {patient.Name}'s history.");
    }

    private void ViewMedicalHistory()
    {
        int? patientId = ReadInt("Patient id: ");
        if (patientId is null) { Invalid("a whole number"); return; }

        Patient patient = _hospital.Registry.GetById(patientId.Value);
        _output.WriteLine($"Medical history for {patient.Name} ({patient.History.Count} records):");
        MedicalHistoryIterator it = patient.History.GetIterator();
        while (it.MoveNext())
            _output.WriteLine("  - " + it.Current);
    }

    private void ChooseBillingStrategy()
    {
        _output.WriteLine("1. Insured (patient pays 20%)");
        _output.WriteLine("2. Uninsured (patient pays full charge)");
        _output.WriteLine("3. Government subsidy (patient pays 40%)");
        IBillingStrategy? strategy = ReadInt("Strategy: ") switch
        {
            1 => new InsuredBilling(),
            2 => new UninsuredBilling(),
            3 => new GovernmentSubsidyBilling(),
            _ => null
        };
        if (strategy is null) { Invalid("1, 2 or 3"); return; }

        _hospital.SetBillingStrategy(strategy);
        _output.WriteLine($"Billing strategy set to: {strategy.Name}");
    }

    private void CalculateBill()
    {
        int? patientId = ReadInt("Patient id: ");
        if (patientId is null) { Invalid("a whole number"); return; }
        decimal? baseCharge = ReadDecimal("Base charge: ");
        if (baseCharge is null or < 0) { Invalid("a non-negative amount"); return; }

        Patient patient = _hospital.Registry.GetById(patientId.Value);
        decimal amount = _hospital.BillPatient(patient, baseCharge.Value);
        _output.WriteLine($"Bill for {patient.Name}: {amount:0.00} " +
                          $"(base {baseCharge.Value:0.00}, {_hospital.BillingStrategy.Name})");
        _output.WriteLine($"Balance due for {patient.Name}: {patient.Account.Balance:0.00}");
    }

    private void RecordPayment()
    {
        int? patientId = ReadInt("Patient id: ");
        if (patientId is null) { Invalid("a whole number"); return; }

        Patient patient = _hospital.Registry.GetById(patientId.Value);
        if (patient.Account.Balance == 0)
        {
            _output.WriteLine($"{patient.Name} has nothing outstanding.");
            return;
        }

        _output.WriteLine($"Balance due: {patient.Account.Balance:0.00}");
        decimal? amount = ReadDecimal("Amount to pay: ");
        if (amount is null or <= 0) { Invalid("an amount greater than zero"); return; }

        _output.WriteLine("1. Cash");
        _output.WriteLine("2. Card");
        _output.WriteLine("3. Bank transfer");
        PaymentMethod? method = ReadInt("Payment method: ") switch
        {
            1 => PaymentMethod.Cash,
            2 => PaymentMethod.Card,
            3 => PaymentMethod.BankTransfer,
            _ => null
        };
        if (method is null) { Invalid("1, 2 or 3"); return; }

        Payment receipt = _hospital.RecordPayment(patient, amount.Value, method.Value);
        _output.WriteLine(receipt.ToReceipt());
    }

    private void ViewAccountStatement()
    {
        int? patientId = ReadInt("Patient id: ");
        if (patientId is null) { Invalid("a whole number"); return; }

        Patient patient = _hospital.Registry.GetById(patientId.Value);
        _output.WriteLine(patient.Account.ToStatement());
    }

    private string ReadText(string prompt)
    {
        _output.Write(prompt);
        return _input.ReadLine()?.Trim() ?? string.Empty;
    }

    private int? ReadInt(string prompt) =>
        int.TryParse(ReadText(prompt), out int value) ? value : null;

    private decimal? ReadDecimal(string prompt) =>
        decimal.TryParse(ReadText(prompt), out decimal value) ? value : null;

    private void Invalid(string expected) =>
        _output.WriteLine($"Invalid input; expected {expected}.");
}
