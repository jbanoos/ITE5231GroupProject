// PHASE 2

namespace HospitalApp.Notifications;

/// <summary>
/// Subject in the Observer pattern. Status changes are staged in a
/// <see cref="Queue{T}"/> and dispatched to every registered observer in FIFO
/// arrival order. <see cref="Notify"/> queues and dispatches immediately;
/// <see cref="QueueChange"/> plus <see cref="DispatchPending"/> allow batched
/// dispatch.
/// </summary>
/// <remarks>
/// Time complexity: QueueChange is O(1) amortised (array-backed queue).
/// DispatchPending is O(k*m) for k pending changes and m observers, since every
/// change is handed to every observer. Subscribe is O(m) because it scans the
/// observer list to reject duplicates, and Unsubscribe is O(m) for the same
/// reason. PendingCount is O(1).
/// </remarks>
public class PatientStatusNotifier
{
    private readonly List<IPatientStatusObserver> _observers = new();
    private readonly Queue<StatusChange> _pending = new();

    public int PendingCount => _pending.Count;

    public void Subscribe(IPatientStatusObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public bool Unsubscribe(IPatientStatusObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        return _observers.Remove(observer);
    }

    /// <summary>Enqueues a status change and immediately flushes the queue. O(m) for m observers.</summary>
    public void Notify(StatusChange change)
    {
        QueueChange(change);
        DispatchPending();
    }

    /// <summary>Enqueues a status change without dispatching it yet. O(1) amortised.</summary>
    public void QueueChange(StatusChange change)
    {
        ArgumentNullException.ThrowIfNull(change);
        _pending.Enqueue(change);
    }

    /// <summary>Dequeues every pending change (FIFO) and broadcasts it to all observers. O(k*m) for k pending changes and m observers.</summary>
    public void DispatchPending()
    {
        while (_pending.Count > 0)
        {
            StatusChange change = _pending.Dequeue();
            foreach (IPatientStatusObserver observer in _observers)
                observer.OnStatusChanged(change);
        }
    }
}
