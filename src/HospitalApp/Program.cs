// PHASE 2

using HospitalApp.Services;
using HospitalApp.UI;

// Interactive menu-driven workflow (Phase 2): admission, triage, treatment,
// appointments, medical history, notifications and billing.
// Replaces the Phase 1 scripted demo, which read no input.

HospitalSystem hospital = HospitalSystem.Instance;
ConsoleMenu menu = new ConsoleMenu(hospital, Console.In, Console.Out);
menu.Run();
