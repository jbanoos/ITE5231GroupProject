// PHASE 1

namespace HospitalApp.Models;

/// <summary>
/// Triage priority. Lower numeric value means higher priority:
/// Critical (1) is treated before Urgent (2), which is treated before Standard (3).
/// </summary>
public enum TriageLevel
{
    Critical = 1,
    Urgent = 2,
    Standard = 3
}
