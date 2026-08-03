# Hospital Patient Management System - Phase 1

Console application in C# (.NET 8, nullable enabled) for the course project. The work is deliberately split into two phases; this folder contains the complete Phase 1.

## Phase 1 scope: core data-management foundation

- **Patient model** (`Models/Patient.cs`) with id, name, age, triage level, and an owned medical history.
- **PatientRegistry** (`Services/PatientRegistry.cs`) - `Dictionary<int, Patient>` for O(1) register/search/update/remove, with duplicate-id protection.
- **TriageQueue** (`DataStructures/TriageQueue.cs`) - custom sorted singly linked list priority queue. Order: Critical (1) before Urgent (2) before Standard (3); FIFO among equal priorities. Enqueue, Dequeue, Peek, Snapshot.
- **AppointmentService** (`Services/AppointmentService.cs`) - one `Stack<Appointment>` per doctor; most recently scheduled appointment is cancelled first (LIFO).
- **MedicalHistory** (`DataStructures/MedicalHistory.cs`) - custom singly linked list of records, plus **MedicalHistoryIterator** (Iterator pattern) for oldest-to-newest traversal with Reset support.
- **HospitalSystem** (`Services/HospitalSystem.cs`) - Singleton facade wiring registry, triage queue, and appointment service together.

## Layout

- `src/HospitalApp/` - console app (`Models/`, `DataStructures/`, `Services/`, `Program.cs` scripted demo; no stdin, exits on its own).
- `tests/HospitalApp.Tests/` - xUnit test project, 19 tests covering registry, triage ordering/FIFO, LIFO appointments, history iteration, and the singleton.

## Run

```
dotnet run --project src/HospitalApp/HospitalApp.csproj
dotnet test tests/HospitalApp.Tests/HospitalApp.Tests.csproj
```

## What Phase 2 will add

- **Observer** pattern with `Queue<StatusChange>` dispatching: notify the console logger, SMS stub, and email stub when a patient's status changes.
- **Strategy** pattern: interchangeable InsuredBilling, UninsuredBilling, and GovernmentSubsidyBilling strategies selectable at runtime.
- Final interactive console workflow (menu-driven admission, triage, treatment, appointments, billing).
- Remaining tests (Observer/Strategy/workflow), GitHub Projects Agile board evidence (see `GITHUB_PROJECT_STEPS.md`), and the final report.
