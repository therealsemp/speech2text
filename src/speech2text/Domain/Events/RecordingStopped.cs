namespace speech2text.Domain.Events;

public record RecordingStopped(DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
