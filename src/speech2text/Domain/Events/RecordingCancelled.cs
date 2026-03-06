namespace speech2text.Domain.Events;

public record RecordingCancelledEvent(DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
