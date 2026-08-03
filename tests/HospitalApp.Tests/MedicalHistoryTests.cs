using HospitalApp.DataStructures;

namespace HospitalApp.Tests;

public class MedicalHistoryTests
{
    [Fact]
    public void Iterator_TraversesRecordsInInsertionOrder()
    {
        var history = new MedicalHistory();
        history.AddRecord("2026-06-01: check-up");
        history.AddRecord("2026-07-15: antibiotics");

        var records = new List<string>();
        MedicalHistoryIterator it = history.GetIterator();
        while (it.MoveNext())
            records.Add(it.Current);

        Assert.Equal(new[] { "2026-06-01: check-up", "2026-07-15: antibiotics" }, records);
        Assert.Equal(2, history.Count);
    }

    [Fact]
    public void Iterator_Reset_AllowsSecondFullPass()
    {
        var history = new MedicalHistory();
        history.AddRecord("record A");
        history.AddRecord("record B");

        MedicalHistoryIterator it = history.GetIterator();
        Assert.True(it.MoveNext());
        Assert.Equal("record A", it.Current);

        it.Reset();

        Assert.True(it.MoveNext());
        Assert.Equal("record A", it.Current);
    }

    [Fact]
    public void Current_BeforeMoveNext_ThrowsInvalidOperationException()
    {
        var history = new MedicalHistory();
        history.AddRecord("record A");

        Assert.Throws<InvalidOperationException>(() => _ = history.GetIterator().Current);
    }

    [Fact]
    public void AddRecord_EmptyString_ThrowsArgumentException()
    {
        var history = new MedicalHistory();

        Assert.Throws<ArgumentException>(() => history.AddRecord("   "));
    }
}
