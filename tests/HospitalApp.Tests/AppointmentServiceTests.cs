// PHASE 1

using HospitalApp.Models;
using HospitalApp.Services;

namespace HospitalApp.Tests;

public class AppointmentServiceTests
{
    // A week out at 09:00: always in the future and inside clinic hours, so the
    // Appointment constructor's date validation accepts it whenever tests run.
    private static Appointment A(string doctor, int patientId) =>
        new(doctor, patientId, DateTime.Today.AddDays(7).AddHours(9), "Check-up");

    [Fact]
    public void CancelMostRecent_ReturnsAppointmentsInLifoOrder()
    {
        var service = new AppointmentService();
        service.Schedule(A("Patel", 1));
        service.Schedule(A("Patel", 2));
        service.Schedule(A("Patel", 3));

        Assert.Equal(3, service.CancelMostRecent("Patel").PatientId);
        Assert.Equal(2, service.CancelMostRecent("Patel").PatientId);
        Assert.Equal(1, service.CancelMostRecent("Patel").PatientId);
        Assert.Equal(0, service.CountFor("Patel"));
    }

    [Fact]
    public void CancelMostRecent_NoAppointments_ThrowsInvalidOperationException()
    {
        var service = new AppointmentService();

        Assert.Throws<InvalidOperationException>(() => service.CancelMostRecent("Patel"));
    }

    [Fact]
    public void Stacks_AreIndependentPerDoctor()
    {
        var service = new AppointmentService();
        service.Schedule(A("Patel", 1));
        service.Schedule(A("Gomez", 2));
        service.Schedule(A("Gomez", 3));

        Assert.Equal(1, service.CountFor("Patel"));
        Assert.Equal(2, service.CountFor("Gomez"));
        Assert.Equal(3, service.CancelMostRecent("Gomez").PatientId);
        Assert.Equal(1, service.PeekNext("Patel")!.PatientId);
    }

    [Fact]
    public void PeekNext_DoesNotRemoveAppointment()
    {
        var service = new AppointmentService();
        service.Schedule(A("Patel", 1));

        Assert.Equal(1, service.PeekNext("Patel")!.PatientId);
        Assert.Equal(1, service.CountFor("Patel"));
        Assert.Null(service.PeekNext("Unknown"));
    }
}
