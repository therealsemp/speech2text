namespace speech2text.Domain.Events;

public abstract record DomainEvent(DateTimeOffset OccurredAt);
