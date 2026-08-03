namespace HospitalApp.DataStructures;

/// <summary>
/// Iterator pattern implementation for <see cref="MedicalHistory"/>.
/// Walks the linked list from the oldest record to the newest.
/// </summary>
public class MedicalHistoryIterator
{
    private readonly MedicalHistory.Node? _head;
    private MedicalHistory.Node? _current;

    internal MedicalHistoryIterator(MedicalHistory.Node? head)
    {
        _head = head;
        _current = null;
    }

    /// <summary>The record at the iterator's current position.</summary>
    public string Current =>
        _current?.Record ?? throw new InvalidOperationException(
            "The iterator is not positioned on a record. Call MoveNext() first.");

    /// <summary>Advances to the next record. Returns false when the end is reached.</summary>
    public bool MoveNext()
    {
        _current = _current is null ? _head : _current.Next;
        return _current is not null;
    }

    /// <summary>Moves the iterator back before the first record.</summary>
    public void Reset() => _current = null;
}
