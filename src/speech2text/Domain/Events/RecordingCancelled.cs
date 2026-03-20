namespace speech2text.Domain.Events;

/// <summary>
/// Emitted when the user presses Escape during recording, transitioning directly from Recording to Idle.
/// No audio is sent for transcription and no text is injected.
/// </summary>
public record RecordingCancelledEvent(DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
