using speech2text.Domain.Events;

namespace speech2text.Domain;

/// <summary>
/// Represents the lifecycle states of a dictation session.
/// </summary>
public enum RecordingState { Idle, Recording, Transcribing }

/// <summary>
/// Central aggregate of the domain. Manages the lifecycle of a single dictation session
/// through a strict state machine: Idle → Recording → Transcribing → Idle.
/// Recording can also be cancelled from the Recording state, returning directly to Idle.
///
/// Each valid transition emits a domain event collected in <see cref="DomainEvents"/>.
/// Invalid transitions (e.g. stopping when already idle) throw <see cref="InvalidOperationException"/>
/// to enforce business invariants.
///
/// The aggregate has no knowledge of audio, network, or UI — it is pure domain logic.
/// </summary>
public class RecordingSession
{
    private readonly List<DomainEvent> _events = [];

    public RecordingState State { get; private set; } = RecordingState.Idle;

    /// <summary>Collected domain events since the last <see cref="ClearEvents"/> call.</summary>
    public IReadOnlyList<DomainEvent> DomainEvents => _events.AsReadOnly();

    /// <summary>Clears the collected domain events. Call after events have been dispatched.</summary>
    public void ClearEvents() => _events.Clear();

    /// <summary>Transitions Idle → Recording. Emits <see cref="RecordingStartedEvent"/>.</summary>
    public void StartRecording()
    {
        if (State != RecordingState.Idle)
            throw new InvalidOperationException($"Cannot start recording in state {State}.");

        State = RecordingState.Recording;
        _events.Add(new RecordingStartedEvent(DateTimeOffset.UtcNow));
    }

    /// <summary>Transitions Recording → Transcribing. Emits <see cref="RecordingStoppedEvent"/>.</summary>
    public void StopRecording()
    {
        if (State != RecordingState.Recording)
            throw new InvalidOperationException($"Cannot stop recording in state {State}.");

        State = RecordingState.Transcribing;
        _events.Add(new RecordingStoppedEvent(DateTimeOffset.UtcNow));
    }

    /// <summary>Transitions Recording → Idle without transcribing. Emits <see cref="RecordingCancelledEvent"/>.</summary>
    public void Cancel()
    {
        if (State != RecordingState.Recording)
            throw new InvalidOperationException($"Cannot cancel in state {State}.");

        State = RecordingState.Idle;
        _events.Add(new RecordingCancelledEvent(DateTimeOffset.UtcNow));
    }

    /// <summary>Transitions Transcribing → Idle. Emits <see cref="TranscriptionCompletedEvent"/> with the transcribed text.</summary>
    public void CompleteTranscription(string text)
    {
        if (State != RecordingState.Transcribing)
            throw new InvalidOperationException($"Cannot complete transcription in state {State}.");

        State = RecordingState.Idle;
        _events.Add(new TranscriptionCompletedEvent(text, DateTimeOffset.UtcNow));
    }
}
