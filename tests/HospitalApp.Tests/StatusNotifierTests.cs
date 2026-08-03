// PHASE 2

using HospitalApp.Models;
using HospitalApp.Notifications;
using HospitalApp.Services;

namespace HospitalApp.Tests;

public class StatusNotifierTests
{
    public StatusNotifierTests() => HospitalSystem.ResetInstance();

    private static StatusChange Sample(
        int id = 1,
        PatientStatus from = PatientStatus.Waiting,
        PatientStatus to = PatientStatus.InExamination) =>
        new(id, "Ava Smith", from, to);

    [Fact]
    public void Notify_BroadcastsChangeToAllSubscribedObservers()
    {
        var notifier = new PatientStatusNotifier();
        var sms = new SmsNotifier(TextWriter.Null);
        var email = new EmailNotifier(TextWriter.Null);
        notifier.Subscribe(sms);
        notifier.Subscribe(email);

        notifier.Notify(Sample());

        Assert.Single(sms.SentMessages);
        Assert.Single(email.SentMessages);
        Assert.Contains("Waiting -> InExamination", sms.SentMessages[0]);
    }

    [Fact]
    public void QueueChange_AccumulatesUntilDispatchPendingFlushesFifo()
    {
        var notifier = new PatientStatusNotifier();
        var sms = new SmsNotifier(TextWriter.Null);
        notifier.Subscribe(sms);

        notifier.QueueChange(Sample(1));
        notifier.QueueChange(Sample(2));

        Assert.Equal(2, notifier.PendingCount);
        Assert.Empty(sms.SentMessages);

        notifier.DispatchPending();

        Assert.Equal(0, notifier.PendingCount);
        Assert.Equal(2, sms.SentMessages.Count);
        Assert.Contains("#1", sms.SentMessages[0]);
        Assert.Contains("#2", sms.SentMessages[1]);
    }

    [Fact]
    public void Unsubscribe_StopsFurtherNotifications()
    {
        var notifier = new PatientStatusNotifier();
        var sms = new SmsNotifier(TextWriter.Null);
        notifier.Subscribe(sms);
        notifier.Notify(Sample());

        Assert.True(notifier.Unsubscribe(sms));
        notifier.Notify(Sample());

        Assert.Single(sms.SentMessages);
    }

    [Fact]
    public void AdmitPatient_BroadcastsWaitingStatusThroughSingletonNotifier()
    {
        HospitalSystem hospital = HospitalSystem.Instance;
        var sms = new SmsNotifier(TextWriter.Null);
        hospital.Notifier.Subscribe(sms);

        hospital.AdmitPatient(1, "Ava Smith", 34, TriageLevel.Standard);

        Assert.Single(sms.SentMessages);
        Assert.Contains("Registered -> Waiting", sms.SentMessages[0]);
    }

    [Fact]
    public void TreatNext_BroadcastsExaminationAndTreatedTransitions()
    {
        HospitalSystem hospital = HospitalSystem.Instance;
        var sms = new SmsNotifier(TextWriter.Null);
        hospital.Notifier.Subscribe(sms);
        hospital.AdmitPatient(1, "Ava Smith", 34, TriageLevel.Critical);

        hospital.TreatNext();

        Assert.Equal(3, sms.SentMessages.Count);
        Assert.Contains("Waiting -> InExamination", sms.SentMessages[1]);
        Assert.Contains("InExamination -> Treated", sms.SentMessages[2]);
    }
}
