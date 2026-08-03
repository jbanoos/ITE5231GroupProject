// PHASE 2

using HospitalApp.Models;
using HospitalApp.Services;
using HospitalApp.UI;

namespace HospitalApp.Tests;

public class WorkflowTests
{
    public WorkflowTests() => HospitalSystem.ResetInstance();

    [Fact]
    public void MenuWorkflow_AdmissionThroughBilling()
    {
        HospitalSystem hospital = HospitalSystem.Instance;
        string script = string.Join('\n',
            "1",         // Admit patient
            "101",       //   id
            "Ava Smith", //   name
            "34",        //   age
            "1",         //   Critical
            "3",         // Treat next patient
            "9",         // Choose billing strategy
            "1",         //   Insured
            "10",        // Calculate bill
            "101",       //   patient id
            "1000",      //   base charge
            "0");        // Exit
        var input = new StringReader(script);
        var output = new StringWriter();
        var menu = new ConsoleMenu(hospital, input, output);

        menu.Run();

        string text = output.ToString();
        Assert.Contains("Admitted: #101 Ava Smith", text);
        Assert.Contains("Treated: #101 Ava Smith", text);
        Assert.Contains("Billing strategy set to: Insured", text);
        Assert.Contains("Bill for Ava Smith: 200.00", text);

        Patient patient = hospital.Registry.GetById(101);
        Assert.Equal(PatientStatus.Treated, patient.Status);
        Assert.Equal(2, patient.History.Count); // treatment note + billing note
    }

    [Fact]
    public void MenuWorkflow_EmptyTriageQueuePrintsErrorAndContinues()
    {
        var input = new StringReader("3\n0\n");
        var output = new StringWriter();
        var menu = new ConsoleMenu(HospitalSystem.Instance, input, output);

        menu.Run();

        Assert.Contains("Error: The triage queue is empty.", output.ToString());
    }
}
