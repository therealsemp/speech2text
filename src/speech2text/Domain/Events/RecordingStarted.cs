namespace speech2text.Domain.Events;

public record RecordingStarted(DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
