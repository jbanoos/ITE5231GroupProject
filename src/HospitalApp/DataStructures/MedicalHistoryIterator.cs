using HospitalApp.Models;

namespace HospitalApp.DataStructures;

/// <summary>
/// Iterator pattern implementation for <see cref="MedicalHistory"/>.
/// Walks the linked list from the oldest record to the newest.
/// </summary>
/// <remarks>
/// Time complexity: MoveNext, Current and Reset are all O(1); a full traversal
/// is O(n). Once MoveNext has returned false the iterator stays exhausted until
/// <see cref="Reset"/> is called, so it never silently restarts.
/// </remarks>
public class MedicalHistoryIterator
{
    private readonly MedicalHistory.Node? _head;
    private MedicalHistory.Node? _current;
    private bool _exhausted;

    internal MedicalHistoryIterator(MedicalHistory.Node? head)
    {
        _head = head;
        Reset();
    }

    /// <summary>The record at the iterator's current position.</summary>
    public MedicalRecord Current =>
        _current?.Record ?? throw new InvalidOperationException(
            "The iterator is not positioned on a record. Call MoveNext() first.");

    /// <summary>Advances to the next record. Returns false when the end is reached.</summary>
    public bool MoveNext()
    {
        if (_exhausted)
            return false;

        // A null _current here can only mean "before the first record", because
        // running off the end is caught by the _exhausted guard above.
        _current = _current is null ? _head : _current.Next;

        if (_current is null)
        {
            _exhausted = true;
            return false;
        }

        return true;
    }

    /// <summary>Moves the iterator back before the first record.</summary>
    public void Reset()
    {
        _current = null;
        _exhausted = false;
    }
}
