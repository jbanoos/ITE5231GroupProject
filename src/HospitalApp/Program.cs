using HospitalApp.DataStructures;
using HospitalApp.Models;
using HospitalApp.Services;

// Phase 1 demo: exercises the core data-management foundation.
// The demo is fully scripted and exits without reading from stdin.

Console.WriteLine("=== Hospital Patient Management System - Phase 1 Demo ===");
Console.WriteLine();

HospitalSystem hospital = HospitalSystem.Instance;

// 1. Admit patients with mixed triage levels (registry + sorted triage queue).
hospital.AdmitPatient(101, "Ava Smith", 34, TriageLevel.Standard);
hospital.AdmitPatient(102, "Ben Cole", 58, TriageLevel.Critical);
hospital.AdmitPatient(103, "Chen Wei", 41, TriageLevel.Urgent);
hospital.AdmitPatient(104, "Dana Ruiz", 29, TriageLevel.Critical);
hospital.AdmitPatient(105, "Eli Novak", 47, TriageLevel.Urgent);

Console.WriteLine($"Registered patients: {hospital.Registry.Count}");
foreach (Patient p in hospital.Registry.GetAll())
    Console.WriteLine("  " + p);
Console.WriteLine();

// 2. Show triage order: Critical first, FIFO among ties.
Console.WriteLine("Triage order (highest priority first):");
foreach (Patient p in hospital.Triage.Snapshot())
    Console.WriteLine("  " + p);
Console.WriteLine();

// 3. Treat the two most urgent patients; each treatment appends to their history.
for (int i = 0; i < 2; i++)
{
    Patient treated = hospital.TreatNext();
    Console.WriteLine($"Treated: {treated}");
}
Console.WriteLine();

// 4. Schedule appointments per doctor (Stack per doctor), then cancel the latest one.
hospital.Appointments.Schedule(new Appointment("Patel", 101, new DateTime(2026, 8, 3, 9, 0, 0), "Follow-up"));
hospital.Appointments.Schedule(new Appointment("Patel", 103, new DateTime(2026, 8, 3, 10, 0, 0), "Lab review"));
hospital.Appointments.Schedule(new Appointment("Gomez", 105, new DateTime(2026, 8, 4, 14, 0, 0), "Consult"));

Console.WriteLine($"Dr. Patel appointments: {hospital.Appointments.CountFor("Patel")}");
Console.WriteLine($"Next for Dr. Patel: {hospital.Appointments.PeekNext("Patel")}");
Appointment cancelled = hospital.Appointments.CancelMostRecent("Patel");
Console.WriteLine($"Cancelled (most recent first): {cancelled}");
Console.WriteLine($"Dr. Patel appointments after cancel: {hospital.Appointments.CountFor("Patel")}");
Console.WriteLine();

// 5. Medical history: add records and walk them with the custom iterator.
Patient? ava = hospital.Registry.GetById(101);
ava.History.AddRecord("2026-06-01: Annual check-up, no issues.");
ava.History.AddRecord("2026-07-15: Prescribed antibiotics for infection.");

Console.WriteLine($"Medical history for {ava.Name}:");
MedicalHistoryIterator it = ava.History.GetIterator();
while (it.MoveNext())
    Console.WriteLine("  - " + it.Current);
Console.WriteLine();

Console.WriteLine("Demo complete.");
