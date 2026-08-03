using HospitalApp.DataStructures;
using HospitalApp.Models;

namespace HospitalApp.Tests;

public class TriageQueueTests
{
    private static Patient P(int id, TriageLevel level) => new(id, $"Patient{id}", 30 + id, level);

    [Fact]
    public void Dequeue_ReturnsCriticalThenUrgentThenStandard()
    {
        var queue = new TriageQueue();
        queue.Enqueue(P(1, TriageLevel.Standard));
        queue.Enqueue(P(2, TriageLevel.Urgent));
        queue.Enqueue(P(3, TriageLevel.Critical));

        Assert.Equal(3, queue.Dequeue().Id);
        Assert.Equal(2, queue.Dequeue().Id);
        Assert.Equal(1, queue.Dequeue().Id);
        Assert.True(queue.IsEmpty);
    }

    [Fact]
    public void Dequeue_SamePriority_IsFifoAmongTies()
    {
        var queue = new TriageQueue();
        queue.Enqueue(P(1, TriageLevel.Urgent));
        queue.Enqueue(P(2, TriageLevel.Critical));
        queue.Enqueue(P(3, TriageLevel.Critical)); // arrived after patient 2
        queue.Enqueue(P(4, TriageLevel.Urgent));   // arrived after patient 1

        // Critical in arrival order, then Urgent in arrival order.
        Assert.Equal(new[] { 2, 3, 1, 4 }, queue.Snapshot().Select(p => p.Id).ToArray());
    }

    [Fact]
    public void Peek_ReturnsHighestPriorityWithoutRemoving()
    {
        var queue = new TriageQueue();
        queue.Enqueue(P(1, TriageLevel.Standard));
        queue.Enqueue(P(2, TriageLevel.Critical));

        Assert.Equal(2, queue.Peek().Id);
        Assert.Equal(2, queue.Count);
        Assert.Equal(2, queue.Peek().Id);
    }

    [Fact]
    public void Dequeue_EmptyQueue_ThrowsInvalidOperationException()
    {
        var queue = new TriageQueue();

        Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
        Assert.Throws<InvalidOperationException>(() => queue.Peek());
    }
}
