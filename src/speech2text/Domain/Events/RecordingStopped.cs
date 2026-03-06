namespace speech2text.Domain.Events;

public record RecordingStoppedEvent(DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
