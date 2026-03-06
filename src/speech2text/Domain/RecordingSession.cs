using speech2text.Domain.Events;

namespace speech2text.Domain;

public enum RecordingState { Idle, Recording, Transcribing }

public class RecordingSession
{
    private readonly List<DomainEvent> _events = [];

    public RecordingState State { get; private set; } = RecordingState.Idle;
    public IReadOnlyList<DomainEvent> DomainEvents => _events.AsReadOnly();

    public void ClearEvents() => _events.Clear();

    public void StartRecording()
    {
        if (State != RecordingState.Idle)
            throw new InvalidOperationException($"Cannot start recording in state {State}.");

        State = RecordingState.Recording;
        _events.Add(new RecordingStarted(DateTimeOffset.UtcNow));
    }

    public void StopRecording()
    {
        if (State != RecordingState.Recording)
            throw new InvalidOperationException($"Cannot stop recording in state {State}.");

        State = RecordingState.Transcribing;
        _events.Add(new RecordingStopped(DateTimeOffset.UtcNow));
    }

    public void Cancel()
    {
        if (State != RecordingState.Recording)
            throw new InvalidOperationException($"Cannot cancel in state {State}.");

        State = RecordingState.Idle;
        _events.Add(new RecordingCancelled(DateTimeOffset.UtcNow));
    }

    public void CompleteTranscription(string text)
    {
        if (State != RecordingState.Transcribing)
            throw new InvalidOperationException($"Cannot complete transcription in state {State}.");

        State = RecordingState.Idle;
        _events.Add(new TranscriptionCompleted(text, DateTimeOffset.UtcNow));
    }
}
