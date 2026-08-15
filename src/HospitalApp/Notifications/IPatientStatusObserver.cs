// PHASE 2

namespace HospitalApp.Notifications;

/// <summary>
/// Observer in the Observer pattern: receives patient status changes
/// dispatched by the PatientStatusNotifier.
/// </summary>
public interface IPatientStatusObserver
{
    void OnStatusChanged(StatusChange change);
}
