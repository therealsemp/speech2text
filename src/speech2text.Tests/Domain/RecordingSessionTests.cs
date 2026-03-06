using speech2text.Domain;
using speech2text.Domain.Events;

namespace speech2text.Tests.Domain;

public class RecordingSessionTests
{
    // --- Initial state ---

    [Fact]
    public void InitialState_IsIdle()
    {
        var session = new RecordingSession();
        Assert.Equal(RecordingState.Idle, session.State);
    }

    [Fact]
    public void InitialState_NoDomainEvents()
    {
        var session = new RecordingSession();
        Assert.Empty(session.DomainEvents);
    }

    // --- Normal flow: Idle → Recording → Transcribing → Idle ---

    [Fact]
    public void StartRecording_FromIdle_TransitionsToRecording()
    {
        var session = new RecordingSession();
        session.StartRecording();
        Assert.Equal(RecordingState.Recording, session.State);
    }

    [Fact]
    public void StartRecording_EmitsRecordingStarted()
    {
        var session = new RecordingSession();
        session.StartRecording();
        Assert.Single(session.DomainEvents);
        Assert.IsType<RecordingStarted>(session.DomainEvents[0]);
    }

    [Fact]
    public void StopRecording_FromRecording_TransitionsToTranscribing()
    {
        var session = new RecordingSession();
        session.StartRecording();
        session.StopRecording();
        Assert.Equal(RecordingState.Transcribing, session.State);
    }

    [Fact]
    public void StopRecording_EmitsRecordingStopped()
    {
        var session = new RecordingSession();
        session.StartRecording();
        session.StopRecording();
        Assert.IsType<RecordingStopped>(session.DomainEvents[1]);
    }

    [Fact]
    public void CompleteTranscription_FromTranscribing_TransitionsToIdle()
    {
        var session = new RecordingSession();
        session.StartRecording();
        session.StopRecording();
        session.CompleteTranscription("hello world");
        Assert.Equal(RecordingState.Idle, session.State);
    }

    [Fact]
    public void CompleteTranscription_EmitsTranscriptionCompletedWithText()
    {
        var session = new RecordingSession();
        session.StartRecording();
        session.StopRecording();
        session.CompleteTranscription("hello world");
        var evt = Assert.IsType<TranscriptionCompleted>(session.DomainEvents[2]);
        Assert.Equal("hello world", evt.Text);
    }

    // --- Cancel flow: Idle → Recording → Idle ---

    [Fact]
    public void Cancel_FromRecording_TransitionsToIdle()
    {
        var session = new RecordingSession();
        session.StartRecording();
        session.Cancel();
        Assert.Equal(RecordingState.Idle, session.State);
    }

    [Fact]
    public void Cancel_EmitsRecordingCancelled()
    {
        var session = new RecordingSession();
        session.StartRecording();
        session.Cancel();
        Assert.IsType<RecordingCancelled>(session.DomainEvents[1]);
    }

    // --- Invalid transitions ---

    [Fact]
    public void StartRecording_WhenAlreadyRecording_Throws()
    {
        var session = new RecordingSession();
        session.StartRecording();
        Assert.Throws<InvalidOperationException>(() => session.StartRecording());
    }

    [Fact]
    public void StartRecording_WhenTranscribing_Throws()
    {
        var session = new RecordingSession();
        session.StartRecording();
        session.StopRecording();
        Assert.Throws<InvalidOperationException>(() => session.StartRecording());
    }

    [Fact]
    public void StopRecording_WhenIdle_Throws()
    {
        var session = new RecordingSession();
        Assert.Throws<InvalidOperationException>(() => session.StopRecording());
    }

    [Fact]
    public void StopRecording_WhenTranscribing_Throws()
    {
        var session = new RecordingSession();
        session.StartRecording();
        session.StopRecording();
        Assert.Throws<InvalidOperationException>(() => session.StopRecording());
    }

    [Fact]
    public void Cancel_WhenIdle_Throws()
    {
        var session = new RecordingSession();
        Assert.Throws<InvalidOperationException>(() => session.Cancel());
    }

    [Fact]
    public void Cancel_WhenTranscribing_Throws()
    {
        var session = new RecordingSession();
        session.StartRecording();
        session.StopRecording();
        Assert.Throws<InvalidOperationException>(() => session.Cancel());
    }

    [Fact]
    public void CompleteTranscription_WhenIdle_Throws()
    {
        var session = new RecordingSession();
        Assert.Throws<InvalidOperationException>(() => session.CompleteTranscription("text"));
    }

    [Fact]
    public void CompleteTranscription_WhenRecording_Throws()
    {
        var session = new RecordingSession();
        session.StartRecording();
        Assert.Throws<InvalidOperationException>(() => session.CompleteTranscription("text"));
    }

    // --- ClearEvents ---

    [Fact]
    public void ClearEvents_RemovesAllEvents()
    {
        var session = new RecordingSession();
        session.StartRecording();
        session.ClearEvents();
        Assert.Empty(session.DomainEvents);
    }
}
