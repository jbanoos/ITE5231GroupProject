# Hospital Patient Management System

**ITE5231: Data Structures & Design Patterns — Project 1**

A console-based hospital management system written in C# on .NET 8. The application registers patients, orders them for treatment by how urgent their condition is, schedules and cancels appointments, keeps a medical history for each patient, notifies staff when a patient's status changes, and calculates and collects payment.

## Team

| Name | Student ID |
|---|---|
| Tu Lien Dang | N01704642 |
| Jan Brian Ano-os | N10003819 |

## Overview

The purpose of this project was to apply core data structures and object-oriented design patterns to a realistic problem rather than to isolated exercises. The system uses six data structures and four Gang-of-Four design patterns, and each was chosen because its defining property matches the behaviour the feature needs.

The application is menu-driven. A user admits patients, views the triage queue, treats the next patient, schedules and cancels appointments, adds and views medical records, selects a billing method, bills a patient, records payments, and views account statements.

Work was completed in two phases. Phase 1 built the data-management foundation. Phase 2 added status notifications, billing, payments, appointment validation, and the interactive console workflow. Every source file carries a `// PHASE 1` or `// PHASE 2` header comment, and Phase 2 additions made inside Phase 1 files are enclosed in matching `// PHASE 2` / `// END OF PHASE 2 BLOCK` markers, so the two stages of work can be told apart without reading the commit history.

## Data structures

| Structure | Where | Why it suits the feature |
|---|---|---|
| Dictionary | `Services/PatientRegistry.cs` | The dominant operation is "find the patient with this ID". Hashing goes straight to a bucket, so lookup, insert and delete are O(1) on average instead of scanning a list. |
| Priority queue (as a sorted linked list) | `DataStructures/TriageQueue.cs` | Triage is a priority problem, not first-come-first-served. Sorting on insert keeps the most urgent patient at the head, so treating the next patient is O(1). |
| Sorted linked list | `DataStructures/TriageQueue.cs` | The same class. The priority queue is implemented by keeping the linked list ordered by triage level at all times. |
| Stack | `Services/AppointmentService.cs` | The requirement is an undo: cancel the last appointment scheduled. Last-in-first-out is the defining property of a stack, so the structure matches the requirement exactly. |
| Linked list | `DataStructures/MedicalHistory.cs` | A history grows one record at a time and is read start to finish. Adding a node needs no resizing or copying, unlike an array-backed list. |
| Queue | `Notifications/PatientStatusNotifier.cs` | Notifications must reach observers in the order the events happened. First-in-first-out is a queue's defining property, so ordering is preserved with no extra work. |

## Design patterns

| Pattern | Where | Problem it solves |
|---|---|---|
| Singleton | `Services/HospitalSystem.cs` | The registry, triage queue, appointments, notifier and billing strategy must be shared. Two separate registries would mean a patient registered in one is invisible to the other. |
| Iterator | `DataStructures/MedicalHistoryIterator.cs` | Callers read a patient's history without knowing it is a linked list. If the storage changed, calling code would not. |
| Observer | `Notifications/PatientStatusNotifier.cs` | A status change must reach several destinations. Adding a new channel means writing a class, not editing `HospitalSystem`. |
| Strategy | `Billing/IBillingStrategy.cs` | Three pricing rules selectable at runtime. Each rule is its own class instead of a growing `if`/`else` chain inside the billing method. |

## Repository structure

```
.
├── docs/
│   ├── written-report.pdf          Written report
│   ├── GitHistory-AgileBoard/      Git history and Agile board evidences
│   └── uml-class-diagram.png       UML class diagram
├── src/HospitalApp/
│   ├── Billing/                    Strategy pattern: billing algorithms
│   │   ├── IBillingStrategy.cs
│   │   ├── InsuredBilling.cs
│   │   ├── UninsuredBilling.cs
│   │   └── GovernmentSubsidyBilling.cs
│   ├── DataStructures/             Structures written by hand
│   │   ├── TriageQueue.cs
│   │   ├── MedicalHistory.cs
│   │   └── MedicalHistoryIterator.cs
│   ├── Models/                     Domain objects and enumerations
│   │   ├── Patient.cs
│   │   ├── MedicalRecord.cs
│   │   ├── Appointment.cs
│   │   ├── PatientAccount.cs
│   │   ├── Payment.cs
│   │   ├── PaymentMethod.cs
│   │   ├── PatientStatus.cs
│   │   └── TriageLevel.cs
│   ├── Notifications/              Observer pattern: subject and observers
│   │   ├── IPatientStatusObserver.cs
│   │   ├── PatientStatusNotifier.cs
│   │   ├── StatusChange.cs
│   │   ├── ConsoleLogger.cs
│   │   ├── SmsNotifier.cs
│   │   └── EmailNotifier.cs
│   ├── Services/                   Registry, appointments, singleton facade
│   │   ├── PatientRegistry.cs
│   │   ├── AppointmentService.cs
│   │   └── HospitalSystem.cs
│   ├── UI/ConsoleMenu.cs           Menu-driven workflow
│   ├── Program.cs                  Entry point
│   └── HospitalApp.csproj
└── tests/HospitalApp.Tests/        xUnit test project (60 tests)
    ├── PatientRegistryTests.cs
    ├── TriageQueueTests.cs
    ├── AppointmentServiceTests.cs
    ├── AppointmentValidationTests.cs
    ├── MedicalHistoryTests.cs
    ├── HospitalSystemTests.cs
    ├── StatusNotifierTests.cs
    ├── BillingStrategyTests.cs
    ├── PatientAccountTests.cs
    ├── WorkflowTests.cs
    ├── AssemblyInfo.cs
    └── HospitalApp.Tests.csproj
```

## Running the application

```
dotnet run --project src/HospitalApp/HospitalApp.csproj
```

The application is menu-driven. Enter an option number and follow the prompts. Choose option `0` to exit.

## Running the tests

```
dotnet test tests/HospitalApp.Tests/HospitalApp.Tests.csproj
```

All 60 tests pass. Test parallelisation is switched off in `AssemblyInfo.cs` because `HospitalSystem` is a singleton and therefore shared state; each test class resets it before running.

| Test class | Tests | Area covered |
|---|---|---|
| `PatientRegistryTests` | 6 | Dictionary add, search, update, delete, duplicate protection |
| `TriageQueueTests` | 4 | Priority ordering and FIFO among equal triage levels |
| `AppointmentServiceTests` | 4 | LIFO cancellation, independent stacks per doctor |
| `MedicalHistoryTests` | 10 | Linked list insertion, iterator traversal and reset |
| `HospitalSystemTests` | 6 | Singleton, admission, treatment, billing, balances |
| `StatusNotifierTests` | 5 | Observer subscribe, unsubscribe, FIFO dispatch |
| `BillingStrategyTests` | 5 | Each billing algorithm and runtime swapping |
| `PatientAccountTests` | 11 | Charges, payments, receipts, statements |
| `AppointmentValidationTests` | 7 | Past dates, advance limit, clinic hours |
| `WorkflowTests` | 2 | End-to-end menu workflow with scripted input |

## Billing, payments and receipts

Every patient owns a `PatientAccount` holding a running outstanding balance and the receipts issued against it.

- `HospitalSystem.BillPatient` prices the base charge with the current billing strategy, adds it to the balance, and notes it in the patient's medical history.
- `HospitalSystem.RecordPayment` settles part or all of the balance and returns a `Payment`, which acts as the receipt: receipt number, timestamp, method, amount, and the balance before and after.
- Receipt numbers follow the form `RCP-0101-002`, built from the patient ID and the number of payments already made on that account. No global counter is needed, and the number is still unique because patient IDs are unique.
- Overpayment is rejected, so a balance can never go negative. Partial payments are supported.

## Appointment scheduling rules

Dates are validated inside the `Appointment` constructor, so an invalid appointment cannot be created regardless of which code path attempts it.

| Rule | Constant |
|---|---|
| Must be in the future | — |
| At most 365 days ahead | `Appointment.MaxAdvanceDays` |
| Start time from 08:00 (inclusive) | `Appointment.OpensAt` |
| Start time before 18:00 (exclusive) | `Appointment.ClosesAt` |

The console menu catches the resulting exception and prints the message, so an invalid date re-prompts instead of crashing the application.

## Time complexity

`n` is the number of items held in the structure concerned. Per-method notes are in the XML documentation comments on each class.

| Structure | Where | Operation | Complexity |
|---|---|---|---|
| Dictionary | `PatientRegistry` | Register / GetById / Update / Remove | O(1) average |
| | | GetAll | O(n) |
| Priority queue (sorted linked list) | `TriageQueue` | Enqueue (sorted insert) | O(n) |
| | | Dequeue / Peek | O(1) |
| | | Snapshot | O(n) |
| Stack | `AppointmentService` | Schedule (push) | O(1) amortised |
| | | CancelMostRecent (pop) / PeekNext | O(1) |
| Queue | `PatientStatusNotifier` | QueueChange / dispatch one change | O(1) amortised |
| Linked list | `MedicalHistory` | AddRecord, newest record | O(1) via tail pointer |
| | | AddRecord, back-dated | O(n) |
| Iterator | `MedicalHistoryIterator` | MoveNext / Current / Reset | O(1) |
| Running total and list | `PatientAccount` | Charge / Pay | O(1) |

Two trade-offs are worth stating:

- **`TriageQueue` pays O(n) on insertion so that removal is O(1).** Sorting on the way in means the highest-priority patient is always at the head, so treating the next patient, the operation that runs most often, is constant time. A binary heap would make both operations O(log n), but the specification requires a sorted linked list.
- **`MedicalHistory` keeps a tail pointer** so the ordinary case, a record dated today and therefore later than everything stored, appends in O(1). Only genuinely back-dated records walk the list.

## Documentation

- `docs/report.docx` — the written report, covering the design, the justification for each data structure and pattern, complexity analysis, and testing.
- `docs/uml-class-diagram.png` — UML class diagram of the completed system, generated from `docs/uml-class-diagram.mmd`.
