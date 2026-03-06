namespace speech2text.Domain.Events;

public record TranscriptionCompletedEvent(string Text, DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
