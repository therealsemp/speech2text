namespace speech2text.Domain.Events;

/// <summary>
/// Emitted when the user activates the hotkey and the session transitions from Idle to Recording.
/// Signals that microphone capture has begun.
/// </summary>
public record RecordingStartedEvent(DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
