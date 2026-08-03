// PHASE 2

using HospitalApp.Models;

namespace HospitalApp.Tests;

public class AppointmentValidationTests
{
    private static Appointment At(DateTime scheduledAt) =>
        new("Patel", 1, scheduledAt, "Check-up");

    private static DateTime NextWeekAt(int hour, int minute = 0) =>
        DateTime.Today.AddDays(7).AddHours(hour).AddMinutes(minute);

    [Fact]
    public void FutureDateInsideClinicHours_IsAccepted()
    {
        DateTime when = NextWeekAt(9, 30);

        Appointment appointment = At(when);

        Assert.Equal(when, appointment.ScheduledAt);
    }

    [Fact]
    public void DateInThePast_ThrowsArgumentOutOfRangeException()
    {
        DateTime yesterday = DateTime.Today.AddDays(-1).AddHours(10);

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => At(yesterday));
        Assert.Contains("already passed", ex.Message);
    }

    [Fact]
    public void DateMoreThanMaxAdvanceDaysAhead_ThrowsArgumentOutOfRangeException()
    {
        DateTime tooFar = DateTime.Today.AddDays(Appointment.MaxAdvanceDays + 5).AddHours(10);

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => At(tooFar));
        Assert.Contains($"{Appointment.MaxAdvanceDays} days", ex.Message);
    }

    [Fact]
    public void TimeBeforeOpening_ThrowsArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => At(NextWeekAt(7)));
        Assert.Contains("outside clinic hours", ex.Message);
    }

    [Fact]
    public void TimeAfterClosing_ThrowsArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => At(NextWeekAt(19)));
        Assert.Contains("outside clinic hours", ex.Message);
    }

    [Fact]
    public void OpeningTime_IsInclusive_ClosingTime_IsExclusive()
    {
        DateTime day = DateTime.Today.AddDays(7);

        Appointment atOpening = At(day + Appointment.OpensAt);
        Assert.Equal(Appointment.OpensAt, atOpening.ScheduledAt.TimeOfDay);

        Assert.Throws<ArgumentOutOfRangeException>(() => At(day + Appointment.ClosesAt));
    }

    [Fact]
    public void InvalidDate_IsRejectedBeforeTheAppointmentIsCreated()
    {
        var service = new HospitalApp.Services.AppointmentService();

        Assert.Throws<ArgumentOutOfRangeException>(() => At(NextWeekAt(20)));
        Assert.Equal(0, service.CountFor("Patel"));
    }
}
