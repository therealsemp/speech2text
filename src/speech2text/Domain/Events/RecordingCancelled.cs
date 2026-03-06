namespace speech2text.Domain.Events;

public record RecordingCancelled(DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
