// PHASE 1

using HospitalApp.Models;

namespace HospitalApp.Services;

/// <summary>
/// Manages appointments as one <see cref="Stack{T}"/> per doctor, so the most
/// recently scheduled appointment for a doctor is the first one cancelled (LIFO).
/// </summary>
/// <remarks>
/// Time complexity: every operation is O(1) — an O(1) dictionary lookup to find
/// the doctor's stack, then an O(1) push, pop or peek. Push is O(1) amortised
/// because the stack occasionally doubles its backing array.
/// </remarks>
public class AppointmentService
{
    private readonly Dictionary<string, Stack<Appointment>> _byDoctor = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Pushes an appointment onto the doctor's stack. O(1) amortised.</summary>
    public void Schedule(Appointment appointment)
    {
        ArgumentNullException.ThrowIfNull(appointment);
        StackFor(appointment.DoctorName).Push(appointment);
    }

    /// <summary>Pops the doctor's most recently scheduled appointment. O(1).</summary>
    public Appointment CancelMostRecent(string doctorName)
    {
        Stack<Appointment> stack = StackFor(doctorName);
        if (stack.Count == 0)
            throw new InvalidOperationException($"Dr. {doctorName} has no appointments to cancel.");
        return stack.Pop();
    }

    /// <summary>Returns the doctor's most recent appointment without removing it, or null if none. O(1).</summary>
    public Appointment? PeekNext(string doctorName) =>
        StackFor(doctorName).TryPeek(out Appointment? appointment) ? appointment : null;

    public int CountFor(string doctorName) => StackFor(doctorName).Count;

    private Stack<Appointment> StackFor(string doctorName)
    {
        if (string.IsNullOrWhiteSpace(doctorName))
            throw new ArgumentException("Doctor name must not be empty.", nameof(doctorName));

        if (!_byDoctor.TryGetValue(doctorName, out Stack<Appointment>? stack))
        {
            stack = new Stack<Appointment>();
            _byDoctor[doctorName] = stack;
        }
        return stack;
    }
}
