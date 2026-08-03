namespace HospitalApp.DataStructures;

/// <summary>
/// A patient's medical history implemented as a custom singly linked list
/// of text records, oldest first. Traversal is done via <see cref="MedicalHistoryIterator"/>.
/// </summary>
public class MedicalHistory
{
    internal sealed class Node
    {
        public string Record { get; }
        public Node? Next { get; set; }

        public Node(string record) => Record = record;
    }

    private Node? _head;
    private Node? _tail;

    public int Count { get; private set; }

    internal Node? Head => _head;

    /// <summary>Appends a record to the end of the history.</summary>
    public void AddRecord(string record)
    {
        if (string.IsNullOrWhiteSpace(record))
            throw new ArgumentException("History record must not be empty.", nameof(record));

        var node = new Node(record);
        if (_tail is null)
        {
            _head = _tail = node;
        }
        else
        {
            _tail.Next = node;
            _tail = node;
        }
        Count++;
    }

    /// <summary>Returns an iterator that walks the records from oldest to newest.</summary>
    public MedicalHistoryIterator GetIterator() => new(_head);
}
