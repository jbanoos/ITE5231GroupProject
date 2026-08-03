using HospitalApp.Models;

namespace HospitalApp.DataStructures;

/// <summary>
/// Triage priority queue implemented as a custom sorted singly linked list.
/// Nodes are kept ordered by <see cref="TriageLevel"/> (Critical=1 first);
/// patients with equal priority are dequeued in FIFO arrival order.
/// </summary>
/// <remarks>
/// Time complexity: Enqueue is O(n) because the list is kept sorted on insert
/// (worst case: a Standard patient walks the whole list). Dequeue and Peek are
/// O(1) since the highest-priority patient is always the head. Snapshot is O(n).
/// Count and IsEmpty are O(1). Keeping the cost on insert is the deliberate
/// trade-off: it makes the treat-next operation, which runs most often, O(1).
/// </remarks>
public class TriageQueue
{
    private sealed class Node
    {
        public Patient Patient { get; }
        public Node? Next { get; set; }

        public Node(Patient patient) => Patient = patient;
    }

    private Node? _head;

    public int Count { get; private set; }
    public bool IsEmpty => Count == 0;

    /// <summary>
    /// Inserts the patient so the list stays sorted by priority. O(n).
    /// The new node is placed after all existing nodes of equal priority,
    /// which preserves FIFO order among ties.
    /// </summary>
    public void Enqueue(Patient patient)
    {
        ArgumentNullException.ThrowIfNull(patient);

        var node = new Node(patient);

        if (_head is null || (int)patient.TriageLevel < (int)_head.Patient.TriageLevel)
        {
            node.Next = _head;
            _head = node;
        }
        else
        {
            Node current = _head;
            while (current.Next is not null &&
                   (int)current.Next.Patient.TriageLevel <= (int)patient.TriageLevel)
            {
                current = current.Next;
            }
            node.Next = current.Next;
            current.Next = node;
        }

        Count++;
    }

    /// <summary>Removes and returns the highest-priority patient. O(1).</summary>
    public Patient Dequeue()
    {
        if (_head is null)
            throw new InvalidOperationException("The triage queue is empty.");

        Patient patient = _head.Patient;
        _head = _head.Next;
        Count--;
        return patient;
    }

    /// <summary>Returns the highest-priority patient without removing them. O(1).</summary>
    public Patient Peek()
    {
        if (_head is null)
            throw new InvalidOperationException("The triage queue is empty.");
        return _head.Patient;
    }

    /// <summary>Returns the patients in the order they would be dequeued. O(n).</summary>
    public IReadOnlyList<Patient> Snapshot()
    {
        var result = new List<Patient>(Count);
        for (Node? current = _head; current is not null; current = current.Next)
            result.Add(current.Patient);
        return result;
    }
}
