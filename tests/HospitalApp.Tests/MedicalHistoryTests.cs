// PHASE 1

using HospitalApp.DataStructures;
using HospitalApp.Models;

namespace HospitalApp.Tests;

public class MedicalHistoryTests
{
    private static MedicalRecord Record(string date, string diagnosis, string? prescription = null) =>
        new(DateTime.Parse(date), diagnosis, prescription);

    [Fact]
    public void Iterator_TraversesRecordsInChronologicalOrder()
    {
        var history = new MedicalHistory();
        history.AddRecord(Record("2026-06-01", "check-up"));
        history.AddRecord(Record("2026-07-15", "chest infection", "antibiotics"));

        var diagnoses = new List<string>();
        MedicalHistoryIterator it = history.GetIterator();
        while (it.MoveNext())
            diagnoses.Add(it.Current.Diagnosis);

        Assert.Equal(new[] { "check-up", "chest infection" }, diagnoses);
        Assert.Equal(2, history.Count);
    }

    [Fact]
    public void AddRecord_BackDatedRecord_IsSplicedIntoChronologicalPosition()
    {
        var history = new MedicalHistory();
        history.AddRecord(Record("2026-06-01", "oldest"));
        history.AddRecord(Record("2026-08-01", "newest"));
        history.AddRecord(Record("2026-07-01", "back-dated"));

        var diagnoses = new List<string>();
        MedicalHistoryIterator it = history.GetIterator();
        while (it.MoveNext())
            diagnoses.Add(it.Current.Diagnosis);

        Assert.Equal(new[] { "oldest", "back-dated", "newest" }, diagnoses);
    }

    [Fact]
    public void AddRecord_RecordOlderThanAllOthers_BecomesTheNewHead()
    {
        var history = new MedicalHistory();
        history.AddRecord(Record("2026-06-01", "second"));
        history.AddRecord(Record("2026-01-01", "first"));

        MedicalHistoryIterator it = history.GetIterator();
        Assert.True(it.MoveNext());
        Assert.Equal("first", it.Current.Diagnosis);
    }

    [Fact]
    public void AddRecord_SameDate_KeepsInsertionOrder()
    {
        var history = new MedicalHistory();
        history.AddRecord(Record("2026-06-01", "added first"));
        history.AddRecord(Record("2026-06-01", "added second"));

        var diagnoses = new List<string>();
        MedicalHistoryIterator it = history.GetIterator();
        while (it.MoveNext())
            diagnoses.Add(it.Current.Diagnosis);

        Assert.Equal(new[] { "added first", "added second" }, diagnoses);
    }

    [Fact]
    public void Iterator_StaysExhausted_AfterReachingTheEnd()
    {
        var history = new MedicalHistory();
        history.AddRecord(Record("2026-06-01", "only record"));

        MedicalHistoryIterator it = history.GetIterator();
        Assert.True(it.MoveNext());
        Assert.False(it.MoveNext());
        Assert.False(it.MoveNext()); // must not silently restart at the head
    }

    [Fact]
    public void Iterator_OnEmptyHistory_ReturnsFalseEveryTime()
    {
        MedicalHistoryIterator it = new MedicalHistory().GetIterator();

        Assert.False(it.MoveNext());
        Assert.False(it.MoveNext());
    }

    [Fact]
    public void Iterator_Reset_AllowsSecondFullPass()
    {
        var history = new MedicalHistory();
        history.AddRecord(Record("2026-06-01", "record A"));
        history.AddRecord(Record("2026-06-02", "record B"));

        MedicalHistoryIterator it = history.GetIterator();
        while (it.MoveNext()) { }

        it.Reset();

        Assert.True(it.MoveNext());
        Assert.Equal("record A", it.Current.Diagnosis);
    }

    [Fact]
    public void Current_BeforeMoveNext_ThrowsInvalidOperationException()
    {
        var history = new MedicalHistory();
        history.AddRecord(Record("2026-06-01", "record A"));

        Assert.Throws<InvalidOperationException>(() => _ = history.GetIterator().Current);
    }

    [Fact]
    public void MedicalRecord_EmptyDiagnosis_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MedicalRecord(DateTime.Now, "   "));
    }

    [Fact]
    public void MedicalRecord_ToString_IncludesPrescriptionOnlyWhenPresent()
    {
        Assert.Equal("2026-06-01: flu", Record("2026-06-01", "flu").ToString());
        Assert.Equal("2026-06-01: flu (Rx: rest)", Record("2026-06-01", "flu", "rest").ToString());
    }
}
