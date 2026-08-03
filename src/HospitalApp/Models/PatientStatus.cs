// PHASE 2

namespace HospitalApp.Models;

/// <summary>
/// Lifecycle status of a patient. Every change is broadcast to all registered
/// observers by the PatientStatusNotifier. New patients start as Registered.
/// </summary>
public enum PatientStatus
{
    Registered,
    Waiting,
    InExamination,
    Treated,
    Discharged
}
