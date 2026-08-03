# Hospital Patient Management System - Phase 1

Console application in C# (.NET 8, nullable enabled) for the course project. The work is deliberately split into two phases; this folder contains the complete Phase 1.

## Phase 1 scope: core data-management foundation

- **Patient model** (`Models/Patient.cs`) with id, name, age, triage level, and an owned medical history.
- **PatientRegistry** (`Services/PatientRegistry.cs`) - `Dictionary<int, Patient>` for O(1) register/search/update/remove, with duplicate-id protection.
- **TriageQueue** (`DataStructures/TriageQueue.cs`) - custom sorted singly linked list priority queue. Order: Critical (1) before Urgent (2) before Standard (3); FIFO among equal priorities. Enqueue, Dequeue, Peek, Snapshot.
- **AppointmentService** (`Services/AppointmentService.cs`) - one `Stack<Appointment>` per doctor; most recently scheduled appointment is cancelled first (LIFO).
- **MedicalHistory** (`DataStructures/MedicalHistory.cs`) - custom singly linked list of `MedicalRecord` nodes (`Models/MedicalRecord.cs`: date, diagnosis, prescription), kept in chronological order so back-dated entries land in the right place. Plus **MedicalHistoryIterator** (Iterator pattern) for oldest-to-newest traversal with Reset support.
- **HospitalSystem** (`Services/HospitalSystem.cs`) - Singleton facade wiring registry, triage queue, and appointment service together.

## Time complexity of each data structure

`n` = items in the structure. Per-method notes are in the XML doc comments on each class.

| Data structure | Where | Operation | Complexity |
|---|---|---|---|
| Dictionary | `PatientRegistry` | Register / GetById / Update / Remove | O(1) average |
| | | GetAll | O(n) |
| Priority queue (sorted linked list) | `TriageQueue` | Enqueue (sorted insert) | O(n) |
| | | Dequeue / Peek | O(1) |
| | | Snapshot | O(n) |
| Stack | `AppointmentService` | Schedule (push) | O(1) amortised |
| | | CancelMostRecent (pop) / PeekNext | O(1) |
| Linked list | `MedicalHistory` | AddRecord, record is newest | O(1) via tail pointer |
| | | AddRecord, back-dated | O(n) |
| Iterator | `MedicalHistoryIterator` | MoveNext / Current / Reset | O(1), O(n) per full pass |

Two notes on the trade-offs:

- **`TriageQueue` pays O(n) on insert to make removal O(1).** Sorting on the way in means the highest-priority patient is always the head, so "treat next patient" — the operation that runs most often — is constant time. A binary heap would make both O(log n), but the assignment specifies a sorted linked list.
- **`MedicalHistory` keeps a tail pointer** so the normal case (a new record dated today, newer than everything already stored) appends in O(1) and only genuinely back-dated records walk the list.

## Layout

- `src/HospitalApp/` - console app (`Models/`, `DataStructures/`, `Services/`, `Program.cs` scripted demo; no stdin, exits on its own).
- `tests/HospitalApp.Tests/` - xUnit test project, 27 tests covering the registry, triage ordering/FIFO, LIFO appointments, chronological history insertion and iteration, and the singleton.

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
