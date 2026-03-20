namespace speech2text.Domain.Events;

/// <summary>
/// Emitted when the transcription service returns a result, transitioning from Transcribing to Idle.
/// Carries the transcribed <see cref="Text"/> that will be injected at the cursor position.
/// </summary>
public record TranscriptionCompletedEvent(string Text, DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
