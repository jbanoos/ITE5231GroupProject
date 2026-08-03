# Hospital Patient Management System

Console application in C# (.NET 8, nullable enabled) for the course project. The work was deliberately split into two phases; this folder contains both phases, complete. Phase 2 additions are marked in the code: new files carry a `// PHASE 2` header, and blocks added to Phase 1 files are wrapped in `// PHASE 2` / `// END OF PHASE 2 BLOCK` comments.

## Phase 1 scope: core data-management foundation

- **Patient model** (`Models/Patient.cs`) with id, name, age, triage level, an owned medical history, and an owned billing account.
- **PatientRegistry** (`Services/PatientRegistry.cs`) - `Dictionary<int, Patient>` for O(1) register/search/update/remove, with duplicate-id protection.
- **TriageQueue** (`DataStructures/TriageQueue.cs`) - custom sorted singly linked list priority queue. Order: Critical (1) before Urgent (2) before Standard (3); FIFO among equal priorities. Enqueue, Dequeue, Peek, Snapshot.
- **AppointmentService** (`Services/AppointmentService.cs`) - one `Stack<Appointment>` per doctor; most recently scheduled appointment is cancelled first (LIFO).
- **MedicalHistory** (`DataStructures/MedicalHistory.cs`) - custom singly linked list of `MedicalRecord` nodes (`Models/MedicalRecord.cs`: date, diagnosis, prescription), kept in chronological order so back-dated entries land in the right place. Plus **MedicalHistoryIterator** (Iterator pattern) for oldest-to-newest traversal with Reset support.
- **HospitalSystem** (`Services/HospitalSystem.cs`) - Singleton facade wiring registry, triage queue, and appointment service together.

## Phase 2 scope: notifications, billing, interactive workflow

- **Observer pattern** (`Notifications/`) - `PatientStatusNotifier` is the subject; it stages `StatusChange` records in a `Queue<StatusChange>` and dispatches them FIFO to all registered observers: `ConsoleLogger`, `SmsNotifier` (stub), and `EmailNotifier` (stub). Status transitions (Registered -> Waiting -> InExamination -> Treated) are broadcast from `HospitalSystem.AdmitPatient`/`TreatNext`.
- **Strategy pattern** (`Billing/`) - `IBillingStrategy` with `InsuredBilling` (patient pays 20%), `UninsuredBilling` (full charge), and `GovernmentSubsidyBilling` (patient pays 40%), swappable at runtime via `HospitalSystem.SetBillingStrategy`.
- **Interactive console workflow** (`UI/ConsoleMenu.cs`, `Program.cs`) - menu-driven admission, triage, treatment, appointments, medical history, billing and payments. Replaces the Phase 1 scripted demo.

## Payments, receipts and balance tracking

Every patient owns a **`PatientAccount`** (`Models/PatientAccount.cs`) holding a running outstanding balance and the receipts issued against it.

- **Billing a patient** (`HospitalSystem.BillPatient`) prices the base charge with the current `IBillingStrategy`, adds the result to the patient's balance, and notes it in their medical history.
- **Recording a payment** (`HospitalSystem.RecordPayment`) settles part or all of the balance and returns a **`Payment`** (`Models/Payment.cs`) - an immutable receipt carrying a receipt number, timestamp, method (Cash / Card / BankTransfer), and the balance before and after.
- Receipt numbers are `RCP-<patient id>-<sequence>`, e.g. `RCP-0101-002`. They are derived from the patient id and the account's own payment count, so they need no global counter and stay unique.
- **Overpayment is rejected**, so a balance can never go negative. Partial payments are supported.
- `PatientAccount.ToStatement()` renders totals plus every receipt; `HospitalSystem.TotalOutstanding()` sums balances across all patients and is shown in the menu header.

## Appointment scheduling rules

Dates are validated in the `Appointment` constructor, so an invalid appointment can never exist regardless of which caller creates it:

| Rule | Constant |
|---|---|
| Must be in the future | — |
| At most 365 days ahead | `Appointment.MaxAdvanceDays` |
| Start time from 08:00 (inclusive) | `Appointment.OpensAt` |
| Start time before 18:00 (exclusive) | `Appointment.ClosesAt` |

The console menu catches the resulting `ArgumentOutOfRangeException` and prints the message, so a bad date re-prompts instead of crashing.

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
| Queue | `PatientStatusNotifier` | QueueChange / Dequeue in DispatchPending | O(1) amortised |
| Linked list | `MedicalHistory` | AddRecord, record is newest | O(1) via tail pointer |
| | | AddRecord, back-dated | O(n) |
| Iterator | `MedicalHistoryIterator` | MoveNext / Current / Reset | O(1), O(n) per full pass |
| Running total + list | `PatientAccount` | Charge / Pay | O(1) |

Two notes on the trade-offs:

- **`TriageQueue` pays O(n) on insert to make removal O(1).** Sorting on the way in means the highest-priority patient is always the head, so "treat next patient" — the operation that runs most often — is constant time. A binary heap would make both O(log n), but the assignment specifies a sorted linked list.
- **`MedicalHistory` keeps a tail pointer** so the normal case (a new record dated today, newer than everything already stored) appends in O(1) and only genuinely back-dated records walk the list.

## Layout

- `src/HospitalApp/` - console app (`Models/`, `DataStructures/`, `Services/`, `Notifications/`, `Billing/`, `UI/`, `Program.cs`).
- `tests/HospitalApp.Tests/` - xUnit test project, 60 tests covering the registry, triage ordering/FIFO, LIFO appointments, chronological history insertion and iteration, the singleton, observer dispatch, billing strategies, account/receipt behaviour, appointment date validation, and the menu workflow. Test parallelization is disabled (`tests/HospitalApp.Tests/AssemblyInfo.cs`) because the singleton is shared state.
- `docs/uml-class-diagram.mmd` - UML class diagram (Mermaid source), with `docs/uml-class-diagram.png` rendered from it.

## Run

```
dotnet run --project src/HospitalApp/HospitalApp.csproj
dotnet test tests/HospitalApp.Tests/HospitalApp.Tests.csproj
```

The app is menu-driven; pick an option number and follow the prompts. Choose option 0 to exit.

## Remaining deliverables (not code)

- GitHub Projects Agile board evidence and the final written report.
