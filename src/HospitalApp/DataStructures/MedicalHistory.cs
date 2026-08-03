// PHASE 1

using HospitalApp.Models;

namespace HospitalApp.DataStructures;

/// <summary>
/// A patient's medical history implemented as a custom singly linked list of
/// <see cref="MedicalRecord"/> nodes kept in chronological order, oldest first.
/// Traversal is done via <see cref="MedicalHistoryIterator"/>.
/// </summary>
/// <remarks>
/// Time complexity: AddRecord is O(1) when the record is not older than the
/// newest one already stored (the normal case, handled by the tail pointer)
/// and O(n) when a back-dated record has to be spliced into the middle.
/// Traversal via the iterator is O(n) overall, O(1) per step. Count is O(1).
/// </remarks>
public class MedicalHistory
{
    internal sealed class Node
    {
        public MedicalRecord Record { get; }
        public Node? Next { get; set; }

        public Node(MedicalRecord record) => Record = record;
    }

    private Node? _head;
    private Node? _tail;

    public int Count { get; private set; }

    internal Node? Head => _head;

    /// <summary>
    /// Inserts a record so the list stays in chronological order. A record
    /// dated on or after the newest existing one is appended in O(1); a
    /// back-dated record is spliced into position. Records sharing a date keep
    /// the order they were added in.
    /// </summary>
    public void AddRecord(MedicalRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        var node = new Node(record);

        if (_head is null)
        {
            _head = _tail = node;
        }
        else if (_tail!.Record.Date <= record.Date)
        {
            _tail.Next = node;
            _tail = node;
        }
        else if (record.Date < _head.Record.Date)
        {
            node.Next = _head;
            _head = node;
        }
        else
        {
            // The tail is newer than the new record, so the walk always stops
            // before the end and the tail pointer stays valid.
            Node current = _head;
            while (current.Next is not null && current.Next.Record.Date <= record.Date)
                current = current.Next;

            node.Next = current.Next;
            current.Next = node;
        }

        Count++;
    }

    /// <summary>Returns an iterator that walks the records from oldest to newest. O(1).</summary>
    public MedicalHistoryIterator GetIterator() => new(_head);
}
