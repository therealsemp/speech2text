namespace speech2text.Domain.Events;

public record RecordingStartedEvent(DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
