namespace speech2text.Domain.Events;

/// <summary>
/// Emitted when the user presses the hotkey a second time, transitioning from Recording to Transcribing.
/// Signals that audio capture has ended and transcription is about to begin.
/// </summary>
public record RecordingStoppedEvent(DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
